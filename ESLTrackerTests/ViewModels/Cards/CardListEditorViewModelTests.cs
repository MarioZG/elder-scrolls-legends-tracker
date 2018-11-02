using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTrackerTests;
using ESLTracker.BusinessLogic.Decks;
using Moq;
using ESLTrackerTests.Builders;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Cards.Tests
{
    [TestClass()]
    public class CardListEditorViewModelTests : BaseTest
    {

        Mock<IDeckService> mockDeckService = new Mock<IDeckService>();
        Mock<IMessenger> mockMessenger = new Mock<IMessenger>();

        [TestMethod()]
        public void AddCardTest001_AddToEmpty()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            CardListEditorViewModel model = CreateListEditorVM();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            model.LimitCardCount = true;

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(2, model.CardsCollection.First().Quantity);
        }

        private CardListEditorViewModel CreateListEditorVM()
        {
            return new CardListEditorViewModel(mockCardsDatabaseFactory.Object, mockDeckService.Object, mockMessenger.Object);
        }

        [TestMethod()]
        public void AddCardTest002_AddAlreadyExisting()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            CardListEditorViewModel model = CreateListEditorVM();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(4, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest003_AddToCOnstructed_Qty3()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            CardListEditorViewModel model = CreateListEditorVM();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            model.LimitCardCount = true;

            model.AddCard(card, 2);

            model.AddCard(card, 1);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(3, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        [Ignore]//TODO: "To be moved to decksergice tests"
        public void AddCardTest004_AddToCOnstructed_ExeedesQty3()
        {
            int maxQty = 3;
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            CardInstance card2 = new CardInstanceBuilder().WithCard(new CardBuilder().WithId(card.CardId).Build()).Build();//in UI other instance of cardinstance is passed
            CardListEditorViewModel model = CreateListEditorVM();
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
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            CardListEditorViewModel model = CreateListEditorVM();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            model.AddCard(card, 2);

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(4, model.CardsCollection.First().Quantity);
        }

        [TestMethod()]
        [Ignore]//TODO: "To be moved to decksergice tests"
        public void AddCardTest006_AddUniqueConstructed()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            CardListEditorViewModel model = CreateListEditorVM();
            model.CardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            model.LimitCardCount = true;
            model.AddCard(card, 2);

            model.AddCard(card, 2);

            Assert.AreEqual(1, model.CardsCollection.Count);
            Assert.AreEqual(1, model.CardsCollection.First().Quantity);
        }
    }
}