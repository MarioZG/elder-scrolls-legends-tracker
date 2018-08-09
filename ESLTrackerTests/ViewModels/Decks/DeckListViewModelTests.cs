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
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.Properties;
using ESLTrackerTests.Builders;
using System.Runtime.Remoting.Messaging;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.DataFile;

namespace ESLTracker.ViewModels.Decks.Tests
{
    [TestClass()]
    public class DeckListViewModelTests : BaseTest
    {
        static IList<Deck> DeckBase;

        Mock<ITracker> tracker = new Mock<ITracker>();
        Mock<IMessenger> mockMessanger = new Mock<IMessenger>();
        Mock<IDeckService> mockDeckService = new Mock<IDeckService>();

        [ClassInitialize()]
        public static void InitTestClass(TestContext tc)
        {

            //init some random classes
            DeckBase = new List<Deck>()
            {
                new DeckBuilder().WithIsHidden(true).WithType(DeckType.Constructed).WithClass(DeckClass.Agility).Build(),
                new DeckBuilder().WithType(DeckType.Constructed).WithClass(DeckClass.Archer).Build(),
                new DeckBuilder().WithIsHidden(true).WithType(DeckType.Constructed).WithClass(DeckClass.Assassin).Build(),
                new DeckBuilder().WithType(DeckType.Constructed).WithClass(DeckClass.Assassin).Build(),
                new DeckBuilder().WithIsHidden(true).WithType(DeckType.Constructed).WithClass(DeckClass.Assassin).Build(),
                new DeckBuilder().WithType(DeckType.Constructed).WithClass(DeckClass.Battlemage).Build(),
                new DeckBuilder().WithIsHidden(true).WithType(DeckType.Constructed).WithClass(DeckClass.Endurance).Build(),
                new DeckBuilder().WithType(DeckType.SoloArena).WithClass(DeckClass.Monk).Build(),
                new DeckBuilder().WithType(DeckType.SoloArena).WithClass(DeckClass.Mage).Build(),
                new DeckBuilder().WithType(DeckType.SoloArena).WithClass(DeckClass.Mage).Build(),
                new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Neutral).Build(),
                new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Neutral).Build(),
                new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Spellsword).Build(),
                new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Strength).Build(),
                new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Willpower).Build(),
            };

            //add each class once
            foreach (DeckClass dc in Enum.GetValues(typeof(DeckClass)))
            {
                DeckBase.Add(new DeckBuilder().WithClass(dc).Build());
            }
        }

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            tracker.Setup(t => t.Games).Returns(new ObservableCollection<DataModel.Game>());
            tracker.Setup(t => t.Decks).Returns(new ObservableCollection<DataModel.Deck>());

        }

        private static Mock<IDeckTypeSelectorViewModel> GetFullTypeFilter()
        {
            List<DeckType> typeFilter = new List<DeckType>(Enum.GetValues(typeof(DeckType)).OfType<DeckType>());
            Mock<IDeckTypeSelectorViewModel> typeSelector = new Mock<IDeckTypeSelectorViewModel>();
            typeSelector.Setup(ts => ts.FilteredTypes).Returns(
                new ObservableCollection<DeckType>(typeFilter));
            typeSelector.Setup(ts => ts.ShowCompletedArenaRuns).Returns(true);
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
            DeckClass selectedClass = DeckClass.Mage;
            var filteredClasses = new ObservableCollection<DeckClass>(new List<DeckClass>() { selectedClass });

            int expectedCount = 3;

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList(
                DeckBase,
                null,
                false,
                false,
                selectedClass,
                filteredClasses,
                null);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.AreEqual(selectedClass, result.ToList()[0].Class);
            Assert.AreEqual(selectedClass, result.ToList()[1].Class);
            Assert.AreEqual(selectedClass, result.ToList()[2].Class);
        }



        [TestMethod()]
        public void FilterDeckListTest002_FilterByOneAttribute()
        {
            DeckAttribute filterAttrib = DeckAttribute.Strength;
            DeckClass? selectedClass = null;
            var filteredClasses =new ObservableCollection<DeckClass>(
                    ClassAttributesHelper.FindClassByAttribute(filterAttrib)
                    );

            Mock<IDeckTypeSelectorViewModel> typeSelector = GetFullTypeFilter();

            int expectedCount = 3 +//random data - archer i battlemage y strength
                5 + //one for every class
                3 ; //hom 3-color classes

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList(
                DeckBase,
                null,
                false,
                false,
                selectedClass,
                filteredClasses,
                null);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsTrue(result.All(r => { return Utils.ClassAttributesHelper.Classes[r.Class.Value].Contains(filterAttrib); }));
        }

        [TestMethod()]
        public void FilterDeckListTest003_FilterByTwoAttributes()
        {
            List<DeckAttribute> filterAttrib = new List<DeckAttribute>()
                { DeckAttribute.Strength, DeckAttribute.Endurance };

            DeckClass? selectedClass = null;
            var filteredClasses = new ObservableCollection<DeckClass>(
                    Utils.ClassAttributesHelper.FindClassByAttribute(filterAttrib)
                    );

            Mock<IDeckTypeSelectorViewModel> typeSelector = GetFullTypeFilter();

            int expectedCount = 0 +//none inrandom data
                1 + //one for every class
                1; //hom 3-color classes

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList(
                DeckBase,
                null,
                false,
                false,
                selectedClass,
                filteredClasses,
                null);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsTrue(result.All(r => { return Utils.ClassAttributesHelper.Classes[r.Class.Value].Contains(filterAttrib[0]); }));

        }


        [TestMethod()]
        public void FilterDeckListTest004_ClearFilters()
        {
            DeckClass? selectedClass = null;
            var filteredClasses = new ObservableCollection<DeckClass>(
                    Utils.ClassAttributesHelper.FindClassByAttribute(DeckAttribute.Neutral)
                    );

            Mock<IDeckTypeSelectorViewModel> typeSelector = GetFullTypeFilter();

            int expectedCount = DeckBase.Count;

            DeckListViewModel model = CreateDeckListVM();


            //do first filter - not intrested
            IEnumerable<Deck> result = model.FilterDeckList(
                DeckBase,
                null,
                false,
                false,
                selectedClass,
                filteredClasses,
                null);

            //model is norw filyred
            Assert.AreNotEqual(expectedCount, result.Count());

            //reset filters - class selector returns all clases
            filteredClasses = new ObservableCollection<DeckClass>(
                        Utils.ClassAttributesHelper.Classes.Keys
                        );

            result = model.FilterDeckList(
                DeckBase,
                null,
                false,
                true,
                selectedClass,
                filteredClasses,
                null);

            Assert.AreEqual(expectedCount, result.Count());
        }

        [TestMethod()]
        public void FilterDeckListTest005_FilterByDeckType()
        {
            List<DeckType> typeFilter = new List<DeckType>() { DeckType.SoloArena };
            var filteredTypes = new ObservableCollection<DeckType>(typeFilter);

            int expectedCount = 3; //only in random data

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList(
                DeckBase, 
                filteredTypes,
                false,
                false,
                null,
                Utils.ClassAttributesHelper.Classes.Keys,
                null);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.IsTrue(result.All(r => { return typeFilter.Contains(r.Type); }));
        }

        [TestMethod()]
        public void FilterDeckListTest006_FilterByClassAndDeckType()
        {
            DeckClass classFilter = DeckClass.Mage;
            var filteredClasses = new ObservableCollection<DeckClass>(new List<DeckClass>() { classFilter });

            List<DeckType> typeFilter = new List<DeckType>() { DeckType.SoloArena };
            var filteredTypes= new ObservableCollection<DeckType>(typeFilter);

            int expectedCount = 2;

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList(
                DeckBase, 
                filteredTypes, 
                false,
                false,
                classFilter, 
                filteredClasses,
                null);

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

            tracker.Setup(t => t.Decks).Returns(new ObservableCollection<Deck>());


            mockSettings.Setup(s => s.DeckViewSortOrder).Returns(DeckViewSortOrder.LastUsed);

            DeckListViewModel model = CreateDeckListVM();

            model.CommandResetFiltersExecute(null);

            //assure filter has been removed
            mockMessanger.Verify(m => m.Send<DeckListResetFilters>(It.IsAny<DeckListResetFilters>(), It.Is<ControlMessangerContext>( c=> c== ControlMessangerContext.DeckList_DeckFilterControl)));
        }

        [TestMethod()]
        public void FilterDeckListTest007_HideCompletedVersusArenaRuns()
        {
            DeckClass classFilter = DeckClass.Mage;
            var selectedClass = classFilter;
            var filteredClasses = new ObservableCollection<DeckClass>(new List<DeckClass>() { classFilter });

            List<DeckType> typeFilter = new List<DeckType>() { DeckType.VersusArena };
            var filteredTypes = new ObservableCollection<DeckType>(typeFilter);
            var showCompletedArenaRuns = false;


            Deck deckToShow = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(classFilter).Build();
            Deck deckToHide = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(classFilter).Build();

            tracker.Setup(t => t.Games).Returns(
                new ObservableCollection<DataModel.Game>(
                    GenerateGamesList(deckToShow, 2, 2).Union(GenerateGamesList(deckToHide, 7, 2))
                ));

            int expectedCount = 1;

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList( 
                new Deck[] { deckToShow, deckToHide },
                filteredTypes, 
                showCompletedArenaRuns,
                false,
                selectedClass, 
                filteredClasses,
                null);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.AreEqual(deckToShow.DeckId, result.First().DeckId);

        }


        [TestMethod()]
        public void FilterDeckListTest008_HideCompletedVersusArenaRunsNoClassFilter()
        {
            DeckClass? selectedClass = null;
            var filteredClasses = new ObservableCollection<DeckClass>(ClassAttributesHelper.Classes.Keys);

            List<DeckType> typeFilter = new List<DeckType>() { DeckType.VersusArena };
            var filteredTypes = new ObservableCollection<DeckType>(typeFilter);
            var showCompletedArenaRuns = false;

            Deck deckToShow = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Assassin).Build();
            Deck deckToHide = new DeckBuilder().WithType(DeckType.VersusArena).WithClass(DeckClass.Inteligence).Build();

            tracker.Setup(t => t.Games).Returns(
                new ObservableCollection<DataModel.Game>(
                    GenerateGamesList(deckToShow, 2, 2).Union(GenerateGamesList(deckToHide, 7, 2))
                ));

            int expectedCount = 1;

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList(
                new Deck[] { deckToShow, deckToHide },
                filteredTypes,
                showCompletedArenaRuns,
                false,
                selectedClass,
                filteredClasses,
                null);

            Assert.AreEqual(expectedCount, result.Count());
            Assert.AreEqual(deckToShow.DeckId, result.First().DeckId);

        }

        [TestMethod()]
        public void FilterDeckListTest009_FilterExcludeHiddenDecks()
        {
            List<DeckType> typeFilter = new List<DeckType>() { DeckType.Constructed };
            var filteredTypes = new ObservableCollection<DeckType>(typeFilter);

            int expectedCount = 24;

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList(
                DeckBase,
                filteredTypes,
                false,
                false,
                null,
                null,
                null);

            Assert.AreEqual(expectedCount, result.Count());
        }

        [TestMethod()]
        public void FilterDeckListTest010_FilterIncludeHiddenDecks()
        {
            List<DeckType> typeFilter = new List<DeckType>() { DeckType.Constructed };
            var filteredTypes = new ObservableCollection<DeckType>(typeFilter);

            int expectedCount = 28;

            DeckListViewModel model = CreateDeckListVM();

            IEnumerable<Deck> result = model.FilterDeckList(
                DeckBase,
                filteredTypes,
                false,
                true,
                null,
                null,
                null);

            Assert.AreEqual(expectedCount, result.Count());
        }

        private DeckListViewModel CreateDeckListVM()
        {
            return new DeckListViewModel(
                mockMessanger.Object, 
                tracker.Object, 
                mockDeckService.Object,
                mockSettings.Object,
                new DeckCalculations(tracker.Object));
        }
    }
}