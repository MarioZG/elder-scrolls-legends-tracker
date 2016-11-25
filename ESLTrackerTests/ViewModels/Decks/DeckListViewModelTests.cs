using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTrackerTests;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using Moq;
using System.Collections.ObjectModel;

namespace ESLTracker.ViewModels.Decks.Tests
{
    [TestClass()]
    public class DeckListViewModelTests : BaseTest
    {
        static IList<Deck> DeckBase;

        [ClassInitialize()]
        public static void InitTestClass(TestContext tc)
        {
            //init some random classes
            DeckBase = new List<Deck>()
            {
                new Deck() {Type = DeckType.Constructed, Class = DeckClass.Agility },
                new Deck() {Type = DeckType.Constructed, Class = DeckClass.Archer },
                new Deck() {Type = DeckType.Constructed, Class = DeckClass.Assassin },
                new Deck() {Type = DeckType.Constructed, Class = DeckClass.Assassin },
                new Deck() {Type = DeckType.Constructed, Class = DeckClass.Assassin },
                new Deck() {Type = DeckType.Constructed, Class = DeckClass.Battlemage},
                new Deck() {Type = DeckType.Constructed, Class = DeckClass.Endurance },
                new Deck() {Type = DeckType.SoloArena, Class = DeckClass.Monk },
                new Deck() {Type = DeckType.SoloArena, Class = DeckClass.Mage },
                new Deck() {Type = DeckType.SoloArena, Class = DeckClass.Mage },
                new Deck() {Type = DeckType.VersusArena, Class = DeckClass.Neutral },
                new Deck() {Type = DeckType.VersusArena, Class = DeckClass.Neutral },
                new Deck() {Type = DeckType.VersusArena, Class = DeckClass.Spellsword },
                new Deck() {Type = DeckType.VersusArena, Class = DeckClass.Strength },
                new Deck() {Type = DeckType.VersusArena, Class = DeckClass.Willpower }
            };

            //add each class once
            foreach (DeckClass dc in Enum.GetValues(typeof(DeckClass)))
            {
                DeckBase.Add(new Deck() { Class = dc });
            }
        }

        private static Mock<IDeckTypeSelectorViewModel> GetFullTypeFilter()
        {
            List<DeckType> typeFilter = new List<DeckType>(Enum.GetValues(typeof(DeckType)).OfType<DeckType>());
            Mock<IDeckTypeSelectorViewModel> typeSelector = new Mock<IDeckTypeSelectorViewModel>();
            typeSelector.Setup(ts => ts.FilteredTypes).Returns(
                new ObservableCollection<DeckType>(typeFilter));
            return typeSelector;
        }

        private static Mock<IDeckClassSelectorViewModel> GetFullClassFilter()
        {
            Mock<IDeckClassSelectorViewModel> classSelector = new Mock<IDeckClassSelectorViewModel>();
            classSelector.Setup(cs => cs.SelectedClass).Returns<DeckClass?>(null);
            classSelector.Setup(cs => cs.FilteredClasses).Returns(
                new ObservableCollection<DeckClass>(
                    Utils.ClassAttributesHelper.Classes.Keys
                    ));
            return classSelector;
        }

        [TestMethod()]
        public void FilterDeckListTest001_FilterByClass()
        {
            DeckClass filter = DeckClass.Mage;
            Mock<IDeckClassSelectorViewModel> classSelector = new Mock<IDeckClassSelectorViewModel>();
            classSelector.Setup(cs => cs.SelectedClass).Returns(filter);
            classSelector.Setup(cs => cs.FilteredClasses).Returns(new ObservableCollection<DeckClass>(new List<DeckClass>() { filter }));

            Mock<IDeckTypeSelectorViewModel> typeSelector = GetFullTypeFilter();

            int expectedCount = 3;

            DeckListViewModel model = new DeckListViewModel();
            model.SetClassFilterViewModel(classSelector.Object);
            model.SetTypeFilterViewModel(typeSelector.Object);

            IEnumerable<Deck> result = model.FilterDeckList(DeckBase);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.AreEqual(filter, result.ToList()[0].Class);
            Assert.AreEqual(filter, result.ToList()[1].Class);
            Assert.AreEqual(filter, result.ToList()[2].Class);
        }



        [TestMethod()]
        public void FilterDeckListTest002_FilterByOneAttribute()
        {
            DeckAttribute filterAttrib = DeckAttribute.Strength;
            //DeckClass filter = DeckClass.Mage;
            Mock<IDeckClassSelectorViewModel> classSelector = new Mock<IDeckClassSelectorViewModel>();
            classSelector.Setup(cs => cs.SelectedClass).Returns<DeckClass?>(null);
            classSelector.Setup(cs => cs.FilteredClasses).Returns(
                new ObservableCollection<DeckClass>(
                    Utils.ClassAttributesHelper.FindClassByAttribute(filterAttrib)
                    ));

            Mock<IDeckTypeSelectorViewModel> typeSelector = GetFullTypeFilter();

            int expectedCount = 3 +//random data - archer i battlemage y strength
                5; //one for every class

            DeckListViewModel model = new DeckListViewModel();
            model.SetClassFilterViewModel(classSelector.Object);
            model.SetTypeFilterViewModel(typeSelector.Object);

            IEnumerable<Deck> result = model.FilterDeckList(DeckBase);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsTrue(result.All(r => { return Utils.ClassAttributesHelper.Classes[r.Class.Value].Contains(filterAttrib); }));
        }

        [TestMethod()]
        public void FilterDeckListTest003_FilterByTwoAttributes()
        {
            List<DeckAttribute> filterAttrib = new List<DeckAttribute>()
            { DeckAttribute.Strength, DeckAttribute.Endurance };
            //DeckClass filter = DeckClass.Mage;
            Mock<IDeckClassSelectorViewModel> classSelector = new Mock<IDeckClassSelectorViewModel>();
            classSelector.Setup(cs => cs.SelectedClass).Returns<DeckClass?>(null);
            classSelector.Setup(cs => cs.FilteredClasses).Returns(
                new ObservableCollection<DeckClass>(
                    Utils.ClassAttributesHelper.FindClassByAttribute(filterAttrib)
                    ));

            Mock<IDeckTypeSelectorViewModel> typeSelector = GetFullTypeFilter();

            int expectedCount = 0 +//none inrandom data
                1; //one for every class

            DeckListViewModel model = new DeckListViewModel();
            model.SetClassFilterViewModel(classSelector.Object);
            model.SetTypeFilterViewModel(typeSelector.Object);

            IEnumerable<Deck> result = model.FilterDeckList(DeckBase);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsTrue(result.All(r => { return Utils.ClassAttributesHelper.Classes[r.Class.Value].Contains(filterAttrib[0]); }));

        }


        [TestMethod()]
        public void FilterDeckListTest004_ClearFilters()
        {
            Mock<IDeckClassSelectorViewModel> classSelector = new Mock<IDeckClassSelectorViewModel>();
            classSelector.Setup(cs => cs.SelectedClass).Returns<DeckClass?>(null);
            classSelector.Setup(cs => cs.FilteredClasses).Returns(
                new ObservableCollection<DeckClass>(
                    Utils.ClassAttributesHelper.FindClassByAttribute(DeckAttribute.Neutral)
                    ));

            Mock<IDeckTypeSelectorViewModel> typeSelector = GetFullTypeFilter();

            int expectedCount = DeckBase.Count;

            DeckListViewModel model = new DeckListViewModel();
            model.SetClassFilterViewModel(classSelector.Object);
            model.SetTypeFilterViewModel(typeSelector.Object);


            //do first filter - not intrested
            IEnumerable<Deck> result = model.FilterDeckList(DeckBase);

            //model is norw filyred
            Assert.AreNotEqual(expectedCount, result.Count());

            //reset filters - class selector returns all clases
            classSelector.Setup(cs => cs.FilteredClasses).Returns(
                    new ObservableCollection<DeckClass>(
                        Utils.ClassAttributesHelper.Classes.Keys
                        ));

            result = model.FilterDeckList(DeckBase);

            Assert.AreEqual(expectedCount, result.Count());
        }

        [TestMethod()]
        public void FilterDeckListTest005_FilterByDeckType()
        {
            Mock<IDeckClassSelectorViewModel> classSelector = GetFullClassFilter();

            List<DeckType> typeFilter = new List<DeckType>() { DeckType.SoloArena };
            Mock<IDeckTypeSelectorViewModel> typeSelector = new Mock<IDeckTypeSelectorViewModel>();
            typeSelector.Setup(ts => ts.FilteredTypes).Returns(
                new ObservableCollection<DeckType>(typeFilter));

            int expectedCount = 3; //only in random data

            DeckListViewModel model = new DeckListViewModel();
            model.SetClassFilterViewModel(classSelector.Object);
            model.SetTypeFilterViewModel(typeSelector.Object);

            IEnumerable<Deck> result = model.FilterDeckList(DeckBase);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsTrue(result.All(r => { return typeFilter.Contains(r.Type); }));
        }

        [TestMethod()]
        public void FilterDeckListTest006_FilterByClassAndDeckType()
        {
            DeckClass classFilter = DeckClass.Mage;
            Mock<IDeckClassSelectorViewModel> classSelector = new Mock<IDeckClassSelectorViewModel>();
            classSelector.Setup(cs => cs.SelectedClass).Returns(classFilter);
            classSelector.Setup(cs => cs.FilteredClasses).Returns(new ObservableCollection<DeckClass>(new List<DeckClass>() { classFilter }));

            List<DeckType> typeFilter = new List<DeckType>() { DeckType.SoloArena };
            Mock<IDeckTypeSelectorViewModel> typeSelector = new Mock<IDeckTypeSelectorViewModel>();
            typeSelector.Setup(ts => ts.FilteredTypes).Returns(
                new ObservableCollection<DeckType>(typeFilter));

            int expectedCount = 2;

            DeckListViewModel model = new DeckListViewModel();
            model.SetClassFilterViewModel(classSelector.Object);
            model.SetTypeFilterViewModel(typeSelector.Object);

            IEnumerable<Deck> result = model.FilterDeckList(DeckBase);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.AreEqual(classFilter, result.ToList()[0].Class);
            Assert.AreEqual(classFilter, result.ToList()[1].Class);
            Assert.AreEqual(typeFilter[0], result.ToList()[0].Type);
            Assert.AreEqual(typeFilter[0], result.ToList()[1].Type);
        }

        [TestMethod()]
        public void ResetFiltersTest001()
        {
            Mock<IDeckClassSelectorViewModel> classSelectorFullFilter = GetFullClassFilter();
            Mock<IDeckTypeSelectorViewModel> typeSelectorFullFilter = GetFullTypeFilter();

            DeckListViewModel model = new DeckListViewModel();
            model.SetClassFilterViewModel(classSelectorFullFilter.Object);
            model.SetTypeFilterViewModel(typeSelectorFullFilter.Object);

            model.CommandResetFiltersExecute(null);

            //assure filter has been removed
            typeSelectorFullFilter.Verify(t => t.Reset());
            classSelectorFullFilter.Verify(t => t.Reset());
        }
    }
}