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
using System.Collections.ObjectModel;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTrackerTests.Builders;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Decks.Tests
{
    [TestClass()]
    public class DeckEditViewModelTests : BaseTest
    {
        ICardInstanceFactory mockCardInstanceFactory = new CardInstanceFactory();
        Mock<IDeckImporter> mockDeckImporter = new Mock<IDeckImporter>();
        Mock<ITracker> mockTracker = new Mock<ITracker>();
        Mock<IMessenger> mockMessenger = new Mock<IMessenger>();
        Mock<IFileSaver> mockFileManager = new Mock<IFileSaver>();
        DeckService mockDeckService;// = new DeckService();
        Mock<IDeckEditImportDeckViewModel> mockDeckEditImportDeckViewModel = new Mock<IDeckEditImportDeckViewModel>();

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            //  mockDeckVersionFactory = new DeckVersionFactory(mockGuidProvider.Object);
            mockDeckService = new DeckService(
                mockTracker.Object,
                mockSettings.Object,
                mockDatetimeProvider.Object, 
                mockGuidProvider.Object);
        }

        [TestMethod]
        public void CancelEditTest001()
        {
            DeckEditViewModel model = CreateDeckEditVM();
            Deck deck = new DeckBuilder().WithDefaultVersion().Build();

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

        private DeckEditViewModel CreateDeckEditVM()
        {
            return new DeckEditViewModel(
                mockLogger.Object,
                mockCardInstanceFactory,
                mockTracker.Object, 
                mockMessenger.Object, 
                mockDatetimeProvider.Object, 
                mockFileManager.Object, 
                mockDeckService,
                mockDeckEditImportDeckViewModel.Object);
        }

        [TestMethod()]
        public void EditDeckStartTest001()
        {
            DeckEditViewModel model = CreateDeckEditVM();
            Deck deck = new DeckBuilder().WithDefaultVersion().Build();

            model.EditDeckStart(new Utils.Messages.EditDeck() { Deck = deck });

            Assert.AreEqual(deck, model.Deck);
            Assert.AreEqual(deck, model.savedState);
        }

     

        [TestMethod()]
        public void SaveDeckTest001_OverwriteCurrent()
        {
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            mockTracker.Setup(t => t.Decks).Returns(deckList);

            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();

            Deck deck = new DeckBuilder().WithName("test deck").WithDefaultVersion().Build();
            deck.SelectedVersion.Cards.Add(card);

            //  List<CardInstance> modifiedCollection = new List<CardInstance>()
            {
                //    card, //include this and ensure it has been cloned
                deck.SelectedVersion.Cards.Add(new CardInstanceBuilder().WithCard(Card.Unknown).Build());
                deck.SelectedVersion.Cards.Add(new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build());
            };

            DeckEditViewModel model = CreateDeckEditVM();
            model.Deck = deck;

            model.SaveDeck(mockTracker.Object, new SerializableVersion(0, 0), deck.SelectedVersion.Cards);

            Assert.AreEqual(3, model.Deck.SelectedVersion.Cards.Count);
            Assert.AreEqual(1, model.Deck.History.Count);
        }

        [TestMethod()]
        public void SaveDeckTest002_SaveMajor()
        {
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            mockTracker.Setup(t => t.Decks).Returns(deckList);


            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();

            Deck deck = new DeckBuilder().WithName("test deck")
                .WithClass(DeckClass.Assassin)
                .WithSelectedVersion(new DeckVersionBuilder().WithVersion(1,0).Build())
                .Build();

            deck.SelectedVersion.Cards.Add(card);

            DeckEditViewModel model = CreateDeckEditVM();
            model.Deck = deck;

            model.BeginEdit();

            deck.SelectedVersion.Cards.Add(new CardInstanceBuilder().WithCard(Card.Unknown).Build());
            deck.SelectedVersion.Cards.Add(new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build());

            model.SaveDeck(mockTracker.Object, new SerializableVersion(1, 0), deck.SelectedVersion.Cards);

            Assert.AreEqual(3, model.Deck.SelectedVersion.Cards.Count);
            Assert.AreEqual(new SerializableVersion(2, 0), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(1, model.Deck.History[0].Cards.Count);
            Assert.AreEqual(2, model.Deck.History.Count);
            //endsure inial card has ebeen cloned
            Assert.IsFalse(model.Deck.SelectedVersion.Cards.Contains(card));
        }

        [TestMethod()]
        public void SaveDeckTest003_SaveMinor()
        {
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            mockTracker.Setup(t => t.Decks).Returns(deckList);

            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();

            Deck deck = new DeckBuilder()
                .WithName("test deck")
                .WithClass(DeckClass.Assassin)
                .WithDefaultVersion()
                .Build();

            deck.SelectedVersion.Cards.Add(card);

            DeckEditViewModel model = CreateDeckEditVM();
            model.Deck = deck;

            model.BeginEdit();

            deck.SelectedVersion.Cards.Add(new CardInstanceBuilder().WithCard(Card.Unknown).Build());
            deck.SelectedVersion.Cards.Add(new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build());

            model.SaveDeck(mockTracker.Object, new SerializableVersion(0, 1), deck.SelectedVersion.Cards);

            Assert.AreEqual(3, model.Deck.SelectedVersion.Cards.Count);
            Assert.AreEqual(new SerializableVersion(1, 1), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(1, model.Deck.History[0].Cards.Count);
            Assert.AreEqual(2, model.Deck.History.Count);
            //endsure inial card has ebeen cloned
            Assert.IsFalse(model.Deck.SelectedVersion.Cards.Contains(card));
        }

        [TestMethod()]
        public void SaveDeckTest004_SaveNotLatestVersion_CrashThatVersionExists()
        {
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            mockTracker.Setup(t => t.Decks).Returns(deckList);


            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();


            DeckVersion dv1 = new DeckVersionBuilder().WithVersion(1, 3).Build(); //ensure its not ordered :)
            DeckVersion dv2 = new DeckVersionBuilder().WithVersion(1, 1).Build(); //ensure its not ordered :)
            DeckVersion dv3 = new DeckVersionBuilder().WithVersion(1, 2).Build(); //ensure its not ordered :)

            Guid selectedVersion = dv2.VersionId;

            Deck deck = new DeckBuilder()
                .WithDefaultVersion()
                .WithName("test deck")
                .WithClass(DeckClass.Assassin)
                .WithVersion(dv1)
                .WithSelectedVersion(dv2)
                .WithVersion(dv3)
                .Build();


            deck.SelectedVersionId = selectedVersion; //select 1.1

            DeckEditViewModel model = CreateDeckEditVM();
            model.Deck = deck;

            model.BeginEdit();

            //tray save as 1.2
            model.SaveDeck(mockTracker.Object, new SerializableVersion(0, 1), deck.SelectedVersion.Cards);

            Assert.AreEqual(new SerializableVersion(1, 4), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(5, model.Deck.History.Count);
        }

        [TestMethod()]
        public void SaveDeckTest005_SaveNotLatestVersion_EnsureNewMajorHaveMinor_0()
        {
            ObservableCollection<Deck> deckList = new ObservableCollection<Deck>();
            mockTracker.Setup(t => t.Decks).Returns(deckList);

            CardInstance card = new CardInstanceBuilder().WithCard(new CardBuilder().Build()).Build();

            DeckVersion dv1 = new DeckVersionBuilder().WithVersion(1, 3).Build(); //ensure its not ordered :)
            DeckVersion dv2 = new DeckVersionBuilder().WithVersion(1, 1).Build(); //ensure its not ordered :)
            DeckVersion dv3 = new DeckVersionBuilder().WithVersion(1, 2).Build(); //ensure its not ordered :)

            Guid selectedVersion = dv2.VersionId;

            Deck deck = new DeckBuilder()
                .WithDefaultVersion()
                .WithName("test deck")
                .WithClass(DeckClass.Assassin)
                .WithVersion(dv1)
                .WithVersion(dv2)
                .WithVersion(dv3)
                .Build();

            deck.SelectedVersionId = selectedVersion; //select 1.1

            DeckEditViewModel model = CreateDeckEditVM();
            model.Deck = deck;

            model.BeginEdit();

            //tray save as 1.2
            model.SaveDeck(mockTracker.Object, new SerializableVersion(1, 0), deck.SelectedVersion.Cards);

            Assert.AreEqual(new SerializableVersion(2, 0), model.Deck.SelectedVersion.Version);
            Assert.AreEqual(5, model.Deck.History.Count);
        }

        [TestMethod()]
        public void CalculateDeckChangesTest001()
        {
            Card c1 = new CardBuilder().WithName("c1").WithId(Guid.NewGuid()).Build();
            Card c2 = new CardBuilder().WithName("c2").WithId(Guid.NewGuid()).Build();
            Card c3 = new CardBuilder().WithName("c3").WithId(Guid.NewGuid()).Build();
            Card c4 = new CardBuilder().WithName("c4").WithId(Guid.NewGuid()).Build();
            Card c5 = new CardBuilder().WithName("c5").WithId(Guid.NewGuid()).Build();

            ObservableCollection<CardInstance> coll1 = new ObservableCollection<CardInstance>();
            coll1.Add(new CardInstanceBuilder().WithCard(c1).WithQuantity(2).Build());
            coll1.Add(new CardInstanceBuilder().WithCard(c2).WithQuantity(2).Build());
            coll1.Add(new CardInstanceBuilder().WithCard(c3).WithQuantity(3).Build());
            coll1.Add(new CardInstanceBuilder().WithCard(c4).WithQuantity(3).Build());

            ObservableCollection<CardInstance> coll2 = new ObservableCollection<CardInstance>();
            coll2.Add(new CardInstanceBuilder().WithCard(c2).WithQuantity(2).Build());
            coll2.Add(new CardInstanceBuilder().WithCard(c3).WithQuantity(1).Build());
            coll2.Add(new CardInstanceBuilder().WithCard(c4).WithQuantity(3).Build());
            coll2.Add(new CardInstanceBuilder().WithCard(c5).WithQuantity(2).Build());

            ObservableCollection<CardInstance> expected = new ObservableCollection<CardInstance>();
            expected.Add(new CardInstanceBuilder().WithCard(c1).WithQuantity(2).Build());
            //c2 - no changes not in a list
            expected.Add(new CardInstanceBuilder().WithCard(c3).WithQuantity(2).Build());
            //c4 - no changes not in a list
            expected.Add(new CardInstanceBuilder().WithCard(c5).WithQuantity(-2).Build());

            var actual = CreateDeckEditVM().CalculateDeckChanges(coll1, coll2);
            CollectionAssert.AreEqual(expected, actual,
                Comparer<CardInstance>.Create((x, y) => x.CardId == y.CardId && x.Quantity == y.Quantity ? 0 : 1),
                "{0}Actual:{0}{1}; {0}Expected:{0}{2}",
                Environment.NewLine,
                String.Join(Environment.NewLine, actual.Select(c => c.DebuggerInfo)),
                String.Join(Environment.NewLine, expected.Select(c => c.DebuggerInfo)));

        }
    }
}