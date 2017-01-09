using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils;
using Moq;
using ESLTracker.DataModel;
using ESLTrackerTests;
using System.Reflection;
using System.Collections;

namespace ESLTracker.ViewModels.Decks.Tests
{
    [TestClass]
    public class DeckPreviewViewModelTests : BaseTest
    {
        [TestMethod]
        public void CancelEditTest001()
        {
            Mock<ITrackerFactory> trackerFactory = new Mock<ITrackerFactory>();
            trackerFactory.Setup(tf => tf.GetNewGuid()).Returns(() => Guid.NewGuid());

            DeckPreviewViewModel model = new DeckPreviewViewModel();
            Deck deck = new Deck(trackerFactory.Object);

            model.Deck = deck;

            PopulateObject(deck, StartProp);
            //fix up selected version id - otherwise it would be some random guid
            deck.History.Last().VersionId = deck.SelectedVersionId;

            TestContext.WriteLine("Begin Edit");
            model.BeginEdit();

            PopulateObject(deck, EditProp);

            TestContext.WriteLine("Cancel Edit");
            model.CancelEdit();

            foreach (PropertyInfo p in deck.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    if (p.PropertyType.GetInterface(nameof(ICollection)) != null)
                    {
                        CollectionAssert.AreEqual(StartProp[p.PropertyType] as ICollection, p.GetValue(deck) as ICollection, "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                    }
                    else
                    {
                        Assert.AreEqual(StartProp[p.PropertyType], p.GetValue(deck), "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                    }
                }
            }
        }

        [TestMethod()]
        public void EditDeckStartTest001()
        {
            Deck deck = Deck.CreateNewDeck();
            DeckPreviewViewModel model = new DeckPreviewViewModel();

            model.EditDeckStart(new Utils.Messages.EditDeck() { Deck = deck });

            Assert.AreEqual(deck, model.Deck);
            Assert.AreEqual(deck, model.savedState);
            Assert.AreEqual(true, model.IsInEditMode);

        }

        [TestMethod()]
        public void GetMaxSingleCardForDeckTest001_Constructed()
        {
            DeckPreviewViewModel model = new DeckPreviewViewModel();

            int? actual = model.GetMaxSingleCardForDeck(new Deck() { Type = DataModel.Enums.DeckType.Constructed });

            Assert.AreEqual(3, actual);

        }

        [TestMethod()]
        public void GetMaxSingleCardForDeckTest001_SoloArena()
        {
            DeckPreviewViewModel model = new DeckPreviewViewModel();

            int? actual = model.GetMaxSingleCardForDeck(new Deck() { Type = DataModel.Enums.DeckType.SoloArena });

            Assert.AreEqual(null, actual);
        }


        [TestMethod()]
        public void GetMaxSingleCardForDeckTest001_VersusArena()
        {
            DeckPreviewViewModel model = new DeckPreviewViewModel();

            int? actual = model.GetMaxSingleCardForDeck(new Deck() { Type = DataModel.Enums.DeckType.VersusArena });

            Assert.AreEqual(null, actual);
        }
    }
}