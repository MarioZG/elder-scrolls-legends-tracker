using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.ViewModels.Cards.Tests
{
    [TestClass()]
    public class CardListEditorViewModelTests
    {
        [TestMethod()]
        public void AddCardTest001_AddToEmpty()
        {
            CardInstance card = new CardInstance(Card.Unknown);
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(2, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest001_AddAlreadyExisting()
        {
            CardInstance card = new CardInstance(Card.Unknown);
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(4, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest001_AddToCOnstructed_Qty3()
        {
            CardInstance card = new CardInstance(Card.Unknown);
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card, 1);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(3, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest001_AddToCOnstructed_ExeedesQty3()
        {
            int maxQty = 3;
            CardInstance card = new CardInstance(Card.Unknown);
            CardInstance card2 = new CardInstance(Card.Unknown);  //in UI other instance of cardinstance is passed
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.MaxSingleCardQuantity = maxQty;

            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card2, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(maxQty, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest001_AddToArena_ExeedesQty3()
        {
            CardInstance card = new CardInstance(Card.Unknown);
            CardListEditorViewModel model = new CardListEditorViewModel();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(4, model.CardsCollection.First().Quantity);
        }
    }
}