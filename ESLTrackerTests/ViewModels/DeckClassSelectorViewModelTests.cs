using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Tests
{
    [TestClass()]
    public class DeckClassSelectorViewModelTests
    {
        [TestMethod()]
        public void ResetTest()
        {
            DeckClassSelectorViewModel model = new DeckClassSelectorViewModel();

            model.FilterClicked(DeckAttribute.Intelligence);

            //has been filter applied?
            Assert.AreNotEqual(Utils.ClassAttributesHelper.Classes.Count, model.FilteredClasses.Count);

            model.Reset();

            //all should be available
            Assert.AreEqual(Utils.ClassAttributesHelper.Classes.Count, model.FilteredClasses.Count);
        }
    }
}