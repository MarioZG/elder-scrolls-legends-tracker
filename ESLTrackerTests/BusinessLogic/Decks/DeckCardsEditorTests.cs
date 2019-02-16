using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.BusinessLogic.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;
using ESLTrackerTests.Builders;
using ESLTracker.BusinessLogic.Cards;
using Moq;

namespace ESLTracker.BusinessLogic.Decks.Tests
{
    [TestClass()]
    public class DeckCardsEditorTests
    {

        private Mock<ICardsDatabase> mockCardsDatabase;
        private CardInstance doubleCard;
        private CardInstance doubleCardComponent1;
        private CardInstance doubleCardComponent2;
        private CardInstance someOtherRandomCard;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            mockCardsDatabase = new Mock<ICardsDatabase>();

            Guid doubleCardId = Guid.Parse("D0000000-e071-4ace-8c3d-c64ba782b2c9");
            Guid doubleCardComponent1Id = Guid.Parse("C1000000-e071-4ace-8c3d-c64ba782b2c9");
            Guid doubleCardComponent2Id = Guid.Parse("C2000000-e071-4ace-8c3d-c64ba782b2c9");
            Guid someOtherRandomCardId = Guid.NewGuid();

            doubleCard = new CardInstanceBuilder().WithCard(
                new CardBuilder()
                .WithId(doubleCardId)
                .WithDoubleCardComponents(
                    new Guid[] { doubleCardComponent1Id, doubleCardComponent2Id }.ToList())
                    .Build()
                ).Build();

            doubleCardComponent1 = new CardInstanceBuilder().WithCard(
                    new CardBuilder()
                    .WithId(doubleCardComponent1Id)
                    .WithDoubleCard(doubleCardId)
                        .Build()
                    ).Build();

            doubleCardComponent2 = new CardInstanceBuilder().WithCard(
                    new CardBuilder()
                    .WithId(doubleCardComponent2Id)
                    .WithDoubleCard(doubleCardId)
                        .Build()
                    ).Build();

            someOtherRandomCard = new CardInstanceBuilder().WithCard(
                    new CardBuilder()
                    .WithId(Guid.NewGuid())
                    .WithDoubleCard(doubleCardId)
                        .Build()
                    ).Build();


            mockCardsDatabase.Setup(cd => cd.FindCardById(doubleCard.CardId)).Returns(doubleCard.Card);
            mockCardsDatabase.Setup(cd => cd.FindCardById(doubleCardComponent1.CardId)).Returns(doubleCardComponent1.Card);
            mockCardsDatabase.Setup(cd => cd.FindCardById(doubleCardComponent2.CardId)).Returns(doubleCardComponent2.Card);
            mockCardsDatabase.Setup(cd => cd.FindCardById(someOtherRandomCard.CardId)).Returns(someOtherRandomCard.Card);
        }

        [TestMethod()]
        public void AddCardTest001_AddToEmpty()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, true);

            Assert.AreEqual(1, cardsCollection.Count);
            Assert.AreEqual(2, cardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest002_AddAlreadyExisting()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, false);

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, false);

            Assert.AreEqual(1, cardsCollection.Count);
            Assert.AreEqual(4, cardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest003_AddToCOnstructed_Qty3()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, true);

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 1, true);

            Assert.AreEqual(1, cardsCollection.Count);
            Assert.AreEqual(3, cardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest004_AddToCOnstructed_ExeedesQty3()
        {
            int maxQty = 3;
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            CardInstance card2 = new CardInstanceBuilder().WithCard(new CardBuilder().WithId(card.CardId).Build()).Build();//in UI other instance of cardinstance is passed
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, true);

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, true);

            Assert.AreEqual(1, cardsCollection.Count);
            Assert.AreEqual(maxQty, cardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest005_AddToArena_ExeedesQty3()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, false);

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, false);

            Assert.AreEqual(1, cardsCollection.Count);
            Assert.AreEqual(4, cardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest006_AddUniqueConstructed()
        {
            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().WithIsUnique(true).Build()).Build();
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();

            cardsEditor.ChangeCardQuantity(cardsCollection, card.Card, 2, true);


            Assert.AreEqual(1, cardsCollection.Count);
            Assert.AreEqual(1, cardsCollection.First().Quantity);
        }

        [TestMethod()]
        public void AddCardTest007_Add_DoubleCard_Parent()
        {
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            int qty = 3;
            cardsEditor.ChangeCardQuantity(cardsCollection, doubleCard.Card, qty, true);


            Assert.AreEqual(2, cardsCollection.Count);
            Assert.AreEqual(null, cardsCollection.Where( c=> c.CardId == doubleCard.CardId).FirstOrDefault()?.Quantity);
            Assert.AreEqual(qty, cardsCollection.Where( c=> c.CardId == doubleCardComponent1.CardId).FirstOrDefault()?.Quantity);
            Assert.AreEqual(qty, cardsCollection.Where( c=> c.CardId == doubleCardComponent2.CardId).FirstOrDefault()?.Quantity);

        }


        [TestMethod()]
        public void AddCardTest008_Add_DoubleCard_Component()
        {
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            int qty = 3;
            cardsEditor.ChangeCardQuantity(cardsCollection, doubleCardComponent1.Card, qty, true);


            Assert.AreEqual(2, cardsCollection.Count);
            Assert.AreEqual(null, cardsCollection.Where(c => c.CardId == doubleCard.CardId).FirstOrDefault()?.Quantity);
            Assert.AreEqual(qty, cardsCollection.Where(c => c.CardId == doubleCardComponent1.CardId).FirstOrDefault()?.Quantity);
            Assert.AreEqual(qty, cardsCollection.Where(c => c.CardId == doubleCardComponent2.CardId).FirstOrDefault()?.Quantity);

        }

        [TestMethod()]
        public void AddCardTest009_Add_DoubleCard_Limit()
        {
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>();
            int qty = 4;

            cardsEditor.ChangeCardQuantity(cardsCollection, doubleCard.Card, qty, true);
            cardsEditor.ChangeCardQuantity(cardsCollection, doubleCardComponent1.Card, qty, true);
            cardsEditor.ChangeCardQuantity(cardsCollection, doubleCardComponent2.Card, qty, true);


            Assert.AreEqual(2, cardsCollection.Count);
            Assert.AreEqual(null, cardsCollection.Where(c => c.CardId == doubleCard.CardId).FirstOrDefault()?.Quantity);
            Assert.AreEqual(3, cardsCollection.Where(c => c.CardId == doubleCardComponent1.CardId).FirstOrDefault()?.Quantity);
            Assert.AreEqual(3, cardsCollection.Where(c => c.CardId == doubleCardComponent2.CardId).FirstOrDefault()?.Quantity);
        }



        [TestMethod()]
        public void RemoveCardTest_001_Remove_DoubleCard()
        {
            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>()
            {
                new CardInstanceBuilder().WithCard(doubleCardComponent1.Card).WithQuantity(3).Build(),
                new CardInstanceBuilder().WithCard(doubleCardComponent2.Card).WithQuantity(3).Build(),
                new CardInstanceBuilder().WithCard(someOtherRandomCard.Card).WithQuantity(3).Build(),
            };

            cardsEditor.ChangeCardQuantity(cardsCollection, doubleCardComponent1.Card, -1, false);

            Assert.AreEqual(3, cardsCollection.Count);
            Assert.AreEqual(2, cardsCollection.Where(c => c.CardId == doubleCardComponent1.CardId).FirstOrDefault()?.Quantity);
            Assert.AreEqual(2, cardsCollection.Where(c => c.CardId == doubleCardComponent1.CardId).FirstOrDefault()?.Quantity);
            Assert.AreEqual(3, cardsCollection.Where(c => c.CardId == someOtherRandomCard.CardId).FirstOrDefault()?.Quantity);
        }

        [TestMethod()]
        public void RemoveCardTest_002_Remove_From_Deck()
        {
            Card card1 = new CardBuilder().WithId(Guid.NewGuid()).Build();
            Card card2 = new CardBuilder().WithId(Guid.NewGuid()).Build();

            DeckCardsEditor cardsEditor = CreateDeckCardsEditor();
            var cardsCollection = new System.Collections.ObjectModel.ObservableCollection<CardInstance>()
            {
                new CardInstanceBuilder().WithCard(card1).WithQuantity(1).Build(),
                new CardInstanceBuilder().WithCard(card2).WithQuantity(2).Build(),
            };

            cardsEditor.ChangeCardQuantity(cardsCollection, card1, -1, false);

            Assert.AreEqual(1, cardsCollection.Count);
            Assert.IsNull(cardsCollection.Where(c => c.CardId == card1.Id).FirstOrDefault());
            Assert.AreEqual(2, cardsCollection.Where(c => c.CardId == card2.Id).FirstOrDefault()?.Quantity);

            cardsEditor.ChangeCardQuantity(cardsCollection, card2, -1, false);

            Assert.AreEqual(1, cardsCollection.Count);
            Assert.IsNull(cardsCollection.Where(c => c.CardId == card1.Id).FirstOrDefault());
            Assert.AreEqual(1, cardsCollection.Where(c => c.CardId == card2.Id).FirstOrDefault()?.Quantity);
        }


        private DeckCardsEditor CreateDeckCardsEditor()
        {
            return new DeckCardsEditor( new CardInstanceFactory(), this.mockCardsDatabase.Object);
        }
    }
}