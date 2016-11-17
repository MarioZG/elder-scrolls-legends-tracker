using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Decks.Tests
{
    [TestClass()]
    public class DeckTypeSelectorViewModelTests
    {
        [TestMethod()]
        public void FilterClickedTest001_NonSelected_ClickOne()
        {
            DeckType clickedType = DeckType.SoloArena;
            int expectedCount = 1;
            List<DeckType> expectedFilter = new List<DeckType>() { clickedType };

            DeckTypeSelectorViewModel model = new DeckTypeSelectorViewModel();
            model.FilterClicked(clickedType);

            Assert.AreEqual(expectedCount, model.FilteredTypes.Count);
            Assert.IsTrue(model.FilteredTypes.All(r => { return expectedFilter.Contains(r); }));

        }

        [TestMethod()]
        public void FilterClickedTest001_OneSelectedSelectOther()
        {
            DeckType clickedType = DeckType.SoloArena;
            DeckType clickedType2 = DeckType.Constructed;
            int expectedCount = 2;
            List<DeckType> expectedFilter = new List<DeckType>() { clickedType, clickedType2 };

            DeckTypeSelectorViewModel model = new DeckTypeSelectorViewModel();
            model.FilterClicked(clickedType);
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

            DeckTypeSelectorViewModel model = new DeckTypeSelectorViewModel();
            foreach (DeckType type in allTypes)
            {
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

            DeckTypeSelectorViewModel model = new DeckTypeSelectorViewModel();
            //first select
            model.FilterClicked(clickedType);

            //ensure filter changed
            Assert.AreEqual(1, model.FilteredTypes.Count);

            //unselect - non-selected
            model.FilterClicked(clickedType);


            Assert.AreEqual(expectedCount, model.FilteredTypes.Count);
            Assert.IsTrue(model.FilteredTypes.All(r => { return expectedFilter.Contains(r); }));
        }
    }
}