using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel.Enums;
using Moq;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Decks.Tests
{
    [TestClass()]
    public class DeckTypeSelectorViewModelTests
    {
        Mock<IMessenger> messenger = new Mock<IMessenger>();

        [TestMethod()]
        public void FilterClickedTest001_NonSelected_ClickOne()
        {
            DeckType clickedType = DeckType.SoloArena;
            int expectedCount = 1;
            List<DeckType> expectedFilter = new List<DeckType>() { clickedType };

            DeckTypeSelectorViewModel model = CreateDeckTypeSelectorVM();
            model.FilterButtonState[clickedType] = true;
            model.FilterClicked(clickedType);

            Assert.AreEqual(expectedCount, model.FilteredTypes.Count);
            Assert.IsTrue(model.FilteredTypes.All(r => { return expectedFilter.Contains(r); }));

        }

        private DeckTypeSelectorViewModel CreateDeckTypeSelectorVM()
        {
            return new DeckTypeSelectorViewModel(messenger.Object);
        }

        [TestMethod()]
        public void FilterClickedTest001_OneSelectedSelectOther()
        {
            DeckType clickedType = DeckType.SoloArena;
            DeckType clickedType2 = DeckType.Constructed;
            int expectedCount = 2;
            List<DeckType> expectedFilter = new List<DeckType>() { clickedType, clickedType2 };

            DeckTypeSelectorViewModel model = CreateDeckTypeSelectorVM();
            model.FilterButtonState[clickedType] = true;
            model.FilterClicked(clickedType);

            model.FilterButtonState[clickedType2] = true;
            model.FilterClicked(clickedType2);

            Assert.AreEqual(expectedCount, model.FilteredTypes.Count);
            Assert.IsTrue(model.FilteredTypes.All(r => { return expectedFilter.Contains(r); }));
        }

        [TestMethod()]
        public void FilterClickedTest001_SelectAllOneByOne()
        {
            IEnumerable<DeckType> allTypes = Enum.GetValues(typeof(DeckType)).OfType<DeckType>();
            int expectedCount = allTypes.Count();
            List<DeckType> expectedFilter = new List<DeckType>(allTypes);

            DeckTypeSelectorViewModel model = CreateDeckTypeSelectorVM();
            foreach (DeckType type in allTypes)
            {
                model.FilterButtonState[type] = true;
                model.FilterClicked(type);
            }

            Assert.AreEqual(expectedCount, model.FilteredTypes.Count);
            Assert.IsTrue(model.FilteredTypes.All(r => { return expectedFilter.Contains(r); }));
        }

        [TestMethod()]
        public void FilterClickedTest001_ClearAfterSelection()
        {
            DeckType clickedType = DeckType.SoloArena;

            IEnumerable<DeckType> allTypes = Enum.GetValues(typeof(DeckType)).OfType<DeckType>();
            int expectedCount = allTypes.Count();
            List<DeckType> expectedFilter = new List<DeckType>(allTypes);

            DeckTypeSelectorViewModel model = CreateDeckTypeSelectorVM();
            //first select
            model.FilterButtonState[clickedType] = true;
            model.FilterClicked(clickedType);

            //ensure filter changed
            Assert.AreEqual(1, model.FilteredTypes.Count);

            //unselect - non-selected
            model.FilterButtonState[clickedType] = false;
            model.FilterClicked(clickedType);


            Assert.AreEqual(expectedCount, model.FilteredTypes.Count);
            Assert.IsTrue(model.FilteredTypes.All(r => { return expectedFilter.Contains(r); }));
        }

        [TestMethod()]
        public void ResetTest()
        {
            DeckTypeSelectorViewModel model = CreateDeckTypeSelectorVM();

            int expectedCount = model.FilteredTypes.Count;

            model.FilteredTypes.RemoveAt(1);
            model.FilterButtonState[DeckType.SoloArena] = true;

            //assure filter modified
            Assert.AreNotEqual(expectedCount, model.FilteredTypes.Count);
            Assert.AreNotEqual(false, model.FilterButtonState[DeckType.SoloArena]);

            model.Reset();

            Assert.AreEqual(expectedCount, model.FilteredTypes.Count);
            Assert.AreEqual(false, model.FilterButtonState[DeckType.SoloArena]);

            //all nclued in filter?
            Assert.IsTrue(Enum.GetValues(typeof(DeckType)).OfType<DeckType>().All(r => { return model.FilteredTypes.Contains(r); }));
        }
    }
}