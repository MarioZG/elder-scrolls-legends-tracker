using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ESLTracker.DataModel;
using System.Collections.ObjectModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using System.Reflection;
using ESLTrackerTests;

namespace ESLTracker.ViewModels.Decks.Tests
{
    [TestClass()]
    public class EditDeckViewModelTests : BaseTest
    {
        [TestMethod]
        public void CommandButtonSaveExecuteTest001_NewDeck()
        {
            Mock<ITracker> tracker = new Mock<ITracker>();
            Mock<IDeckClassSelectorViewModel> deckClass = new Mock<IDeckClassSelectorViewModel>();


            DeckClass deckClassValue = DeckClass.Mage;
            string deckName = "Test deck";
            DeckType deckType = DeckType.VersusArena;
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>();

            tracker.Setup(t => t.Decks).Returns(decks);
            deckClass.Setup(d => d.SelectedClass).Returns(deckClassValue);

            EditDeckViewModel model = new EditDeckViewModel();
            model.DeckClassModel = deckClass.Object;
            model.Deck.Name = deckName;
            model.Deck.Type = deckType;


            model.SaveDeck(
                deckClass.Object, 
                tracker.Object
                );

            //check if added correctly
            Assert.AreEqual(1, tracker.Object.Decks.Count);
            Assert.AreEqual(deckClassValue, tracker.Object.Decks[0].Class);
            Assert.AreEqual(deckName, tracker.Object.Decks[0].Name);
            Assert.AreEqual(deckType, tracker.Object.Decks[0].Type);
        }


        [TestMethod]
        public void CommandButtonSaveExecuteTest002_EditDeck()
        {
            Mock<ITracker> tracker = new Mock<ITracker>();
            Mock<IDeckClassSelectorViewModel> deckClass = new Mock<IDeckClassSelectorViewModel>();


            DeckClass deckClassValue = DeckClass.Mage;
            string deckName = "Test deck";
            string deckNameAfterChange = "Test deck changed";
            DeckType deckType = DeckType.VersusArena;
            ObservableCollection<Deck> decks = new ObservableCollection<Deck>();
            //add some decks
            decks.Add(new Deck());
            decks.Add(new Deck());
            decks.Add(new Deck());
            decks.Add(new Deck());

            Deck editedDeck = new Deck()
            {
                Name = deckName,
                Class = deckClassValue,
                Attributes = ClassAttributesHelper.Classes[DeckClass.Spellsword], //add some random attribs, to ensure change is handled properly
                Type = deckType
            };
            decks.Add(editedDeck);

            int expectedDeckCount = decks.Count;


            tracker.Setup(t => t.Decks).Returns(decks);
            deckClass.Setup(d => d.SelectedClass).Returns(deckClassValue);

            EditDeckViewModel model = new EditDeckViewModel();
            model.DeckClassModel = deckClass.Object;
            model.Deck = editedDeck;
            model.Deck.Name = deckNameAfterChange;


            model.SaveDeck(
                deckClass.Object,
                tracker.Object
                );

            //check if added correctly
            Assert.AreEqual(expectedDeckCount, tracker.Object.Decks.Count);

            Deck editedDeckInTracker = tracker.Object.Decks.Where(d => d.DeckId == editedDeck.DeckId).FirstOrDefault();
            Assert.IsNotNull(editedDeckInTracker);
            Assert.AreEqual(deckClassValue, editedDeckInTracker.Class);
            Assert.AreEqual(deckNameAfterChange, editedDeckInTracker.Name);
            Assert.AreEqual(deckType, editedDeckInTracker.Type);
            CollectionAssert.AreEqual(ClassAttributesHelper.Classes[deckClassValue], editedDeckInTracker.Attributes);
        }

        [TestMethod]
        public void IEditableObjectImplementation001_CancelEdit()
        {
            Dictionary<Type, object> startProp = new Dictionary<Type, object>()
            {
                { typeof(Guid), Guid.NewGuid() },
                { typeof(string), "start value" },
                { typeof(DeckType), DeckType.SoloArena },
                { typeof(DeckAttributes), new DeckAttributes() { DeckAttribute.Intelligence } },
                { typeof(DeckClass?), DeckClass.Crusader },
                { typeof(DateTime), DateTime.Now },
                { typeof(object), new object()}
            };
            Dictionary<Type, object> editProp = new Dictionary<Type, object>()
            {
                { typeof(Guid), Guid.NewGuid() },
                { typeof(string), "modified value" },
                { typeof(DeckType), DeckType.VersusArena },
                { typeof(DeckAttributes), new DeckAttributes() { DeckAttribute.Endurance, DeckAttribute.Strength } },
                { typeof(DeckClass?), DeckClass.Monk },
                { typeof(DateTime), DateTime.Now.AddDays(-4) }
             //   { typeof(object), new object()}
            };

            Mock<IDeckClassSelectorViewModel> deckClassSelector = new Mock<IDeckClassSelectorViewModel>();

            EditDeckViewModel model = new EditDeckViewModel();
            model.DeckClassModel = deckClassSelector.Object;
            Deck deck = new Deck();

            model.Deck = deck;

            foreach (PropertyInfo p in deck.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    TestContext.WriteLine("Setting prop {0} of type {1}", p.Name, p.PropertyType);
                    p.SetValue(deck, startProp[p.PropertyType]);
                }
            }

            TestContext.WriteLine("Begin Edit");
            model.BeginEdit();

            foreach (PropertyInfo p in deck.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    TestContext.WriteLine("Setting prop {0} of type {1}", p.Name, p.PropertyType);
                    p.SetValue(deck, editProp[p.PropertyType]);
                }
            }

            TestContext.WriteLine("Cancel Edit");
            model.CancelEdit();

            foreach (PropertyInfo p in deck.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    Assert.AreEqual(startProp[p.PropertyType], p.GetValue(deck), "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                }
            }

        }
    }
}