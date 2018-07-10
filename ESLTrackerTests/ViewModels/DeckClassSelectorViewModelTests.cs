using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using Moq;
using ESLTracker.Services;

namespace ESLTracker.ViewModels.Tests
{
    [TestClass()]
    public class DeckClassSelectorViewModelTests
    {
        Mock<IMessenger> messenger = new Mock<IMessenger>();

        [TestMethod()]
        public void ResetTest()
        {
            DeckClassSelectorViewModel model = CreateClassSelectorVM();

            model.FilterClicked(DeckAttribute.Intelligence);

            //has been filter applied?
            Assert.AreNotEqual(Utils.ClassAttributesHelper.Classes.Count, model.FilteredClasses.Count);

            model.Reset();

            //all should be available
            Assert.AreEqual(Utils.ClassAttributesHelper.Classes.Count, model.FilteredClasses.Count);
        }



        [TestMethod()]
        public void FilterCombo001_WhenFilteredSelectFirstClass_NoFilter()
        {
            DeckClassSelectorViewModel model = CreateClassSelectorVM();

            DeckClass? expected = null;

            model.FilterCombo();

            //none selcted
            Assert.AreEqual(expected, model.SelectedClass);
        }

        [TestMethod()]
        public void FilterCombo002_WhenFilteredSelectFirstClass_OneSelected()
        {
            DeckClassSelectorViewModel model = CreateClassSelectorVM();

            DeckClass? expected = DeckClass.Inteligence;

            model.FilterClicked(DeckAttribute.Intelligence);

            //has been filter applied?
            Assert.AreNotEqual(Utils.ClassAttributesHelper.Classes.Count, model.FilteredClasses.Count);


            model.FilterCombo();

            //none selcted
            Assert.AreEqual(expected, model.SelectedClass);
        }

        [TestMethod()]
        public void FilterCombo003_WhenFilteredSelectFirstClass_TwoSelected()
        {
            DeckClassSelectorViewModel model = CreateClassSelectorVM();

            DeckClass? expected = DeckClass.Battlemage;

            model.FilterClicked(DeckAttribute.Intelligence);
            model.FilterClicked(DeckAttribute.Strength);

            //has been filter applied?
            Assert.AreNotEqual(Utils.ClassAttributesHelper.Classes.Count, model.FilteredClasses.Count);


            model.FilterCombo();

            //none selcted
            Assert.AreEqual(expected, model.SelectedClass);
        }

        [TestMethod()]
        public void FilterCombo004_WhenFilteredSelectFirstClass_ThreeSelected()
        {
            DeckClassSelectorViewModel model = CreateClassSelectorVM();

            DeckClass? expected = DeckClass.Dagoth;

            model.FilterClicked(DeckAttribute.Intelligence);
            model.FilterClicked(DeckAttribute.Strength);
            model.FilterClicked(DeckAttribute.Agility);

            //has been filter applied?
            Assert.AreNotEqual(Utils.ClassAttributesHelper.Classes.Count, model.FilteredClasses.Count);


            model.FilterCombo();

            //none selcted
            Assert.AreEqual(expected, model.SelectedClass);
        }

        [TestMethod()]
        public void DeckClassSelectorViewModelTest001_SelectThreeAttributes()
        {
            DeckClassSelectorViewModel model = CreateClassSelectorVM();

            model.FilterClicked(DeckAttribute.Intelligence);
            model.FilterClicked(DeckAttribute.Strength);
            model.FilterClicked(DeckAttribute.Agility);

            Assert.AreEqual(true, model.FilterButtonState[DeckAttribute.Intelligence]);
            Assert.AreEqual(true, model.FilterButtonState[DeckAttribute.Strength]);
            Assert.AreEqual(true, model.FilterButtonState[DeckAttribute.Agility]);
            Assert.AreEqual(3, model.FilterButtonStateCollection.Count);
            Assert.AreEqual(DeckClass.Dagoth, model.SelectedClass);
        }

        [TestMethod()]
        public void DeckClassSelectorViewModelTest002_SelectAttributesAndSetNull()
        {
            DeckClassSelectorViewModel model = CreateClassSelectorVM();

            model.FilterClicked(DeckAttribute.Intelligence);
            model.FilterClicked(DeckAttribute.Strength);

            Assert.AreEqual(true, model.FilterButtonState[DeckAttribute.Intelligence]);
            Assert.AreEqual(true, model.FilterButtonState[DeckAttribute.Strength]);

            Assert.IsNotNull(model.SelectedClass);

            //assign selected class null from external control (after game add)
            model.SelectedClass = null;

            Assert.AreEqual(0, model.FilterButtonStateCollection.Count);


        }

        private DeckClassSelectorViewModel CreateClassSelectorVM()
        {
            return new DeckClassSelectorViewModel(messenger.Object);
        }
    }
}