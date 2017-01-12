using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTrackerTests;

namespace ESLTracker.ViewModels.Cards.Tests
{
    [TestClass()]
    public class CardListEditorViewModelTests : BaseTest
    {
        [TestMethod()]
        public void AddCardTest001_AddToEmpty()
        {
            CardInstance card = new CardInstance(new Card());
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            model.LimitCardCount = true;

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(2, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest002_AddAlreadyExisting()
        {
            CardInstance card = new CardInstance(new Card());
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(4, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest003_AddToCOnstructed_Qty3()
        {
            CardInstance card = new CardInstance(new Card());
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            model.LimitCardCount = true;

            model.AddCard(card, 2);

            model.AddCard(card, 1);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(3, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest004_AddToCOnstructed_ExeedesQty3()
        {
            int maxQty = 3;
            CardInstance card = new CardInstance(new Card());
            CardInstance card2 = new CardInstance(new Card() { Id = card.CardId });  //in UI other instance of cardinstance is passed
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.LimitCardCount = true;

            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card2, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(maxQty, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest005_AddToArena_ExeedesQty3()
        {
            CardInstance card = new CardInstance(new Card());
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(4, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest006_AddUniqueConstructed()
        {
            CardInstance card = new CardInstance(new Card());
            card.Card.IsUnique = true;

            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            model.LimitCardCount = true;
            model.AddCard(card, 2);

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(1, model.CardsCollection.First().Quantity);
        }
    }
}