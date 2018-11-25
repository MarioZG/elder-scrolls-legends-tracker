using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.DiagnosticsWrappers;
using ESLTracker.ViewModels.Decks;
using ESLTrackerTests.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils.Messages;

namespace ESLTrackerTests.ViewModels.Decks
{
    [TestClass]
    public class DeckItemMenuOperationsViewModelTests : BaseTest
    {

        Mock<IDeckService> mockDeckService;
        Mock<IMessenger> mockMessanger;
        Mock<IFileSaver> mockfileSaver;
        Mock<ITracker> mockTracker;
        Mock<IProcessWrapper> mockProcessWrapper;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            mockDeckService = new Mock<IDeckService>();
            mockMessanger = new Mock<IMessenger>();
            mockfileSaver = new Mock<IFileSaver>();
            mockTracker = new Mock<ITracker>();
            mockProcessWrapper = new Mock<IProcessWrapper>();
        }

        [TestMethod()]
        public void NewDeckTest()
        {
            Deck newDeck = new DeckBuilder().Build();

            mockDeckService.Setup(ds => ds.CreateNewDeck(It.IsAny<string>())).Returns(newDeck);

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.NewDeck(null);

            mockMessanger.Verify(messanger =>
                messanger.Send(
                    It.Is<EditDeck>(m => m.Deck == newDeck), 
                    It.Is<EditDeck.Context>(c=> c == EditDeck.Context.StartEdit)
                    ), 
                Times.Once);

        }

        [TestMethod()]
        public void CommandEditDeckExecute()
        {
            Deck deck = new DeckBuilder().Build();

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.CommandEditDeckExecute(deck);

            mockMessanger.Verify(messanger =>
                messanger.Send(
                    It.Is<EditDeck>(m => m.Deck == deck),
                    It.Is<EditDeck.Context>(c => c == EditDeck.Context.StartEdit)
                    ),
                Times.Once);

        }

        [TestMethod()]
        public void CommandHideDeckExecute()
        {
            Deck deck = new DeckBuilder().Build();

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.CommandHideDeckExecute(deck);

            Assert.AreEqual(true, deck.IsHidden);

            mockfileSaver.Verify(ms => ms.SaveDatabase(mockTracker.Object), Times.Once);
            


        }

        [TestMethod()]
        public void CommandUnHideDeckExecute()
        {
            Deck deck = new DeckBuilder().Build();

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.CommandUnHideDeckExecute(deck);

            Assert.AreEqual(false, deck.IsHidden);

            mockfileSaver.Verify(ms => ms.SaveDatabase(mockTracker.Object), Times.Once);

        }

        [TestMethod()]
        public void CommandDeleteDeckExecute001_CanDelete()
        {
            Deck deck = new DeckBuilder().Build();

            mockDeckService.Setup(ds => ds.CanDelete(deck)).Returns(true);

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.CommandDeleteDeckExecute(deck);

            mockDeckService.Verify(ds => ds.DeleteDeck(deck), Times.Once);
            mockfileSaver.Verify(ms => ms.SaveDatabase(mockTracker.Object), Times.Once);

        }

        [TestMethod()]
        public void CommandDeleteDeckExecute002_Cannotdelete()
        {
            Deck deck = new DeckBuilder().Build();

            mockDeckService.Setup(ds => ds.CanDelete(deck)).Returns(false);

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.CommandDeleteDeckExecute(deck);

            mockDeckService.Verify(ds => ds.DeleteDeck(deck), Times.Never);
            mockfileSaver.Verify(ms => ms.SaveDatabase(mockTracker.Object), Times.Never);

        }

        [TestMethod()]
        public void CommandOpenUrlExecute()
        {
            string deckUrl = "uf test 123";
            Deck deck = new DeckBuilder().WithUrl(deckUrl).Build();

            mockDeckService.Setup(ds => ds.CanDelete(deck)).Returns(false);

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.CommandOpenUrlExecute(deck);

            mockProcessWrapper.Verify(pw => pw.Start(deckUrl), Times.Once);

        }

        private DeckItemMenuOperationsViewModel CreateDeckItemMenuOperationsViewModelObject()
        {
            return new DeckItemMenuOperationsViewModel(
                mockMessanger.Object,
                mockfileSaver.Object,
                mockTracker.Object,
                mockDeckService.Object,
                mockProcessWrapper.Object);
        }
    }
}
