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
using ESLTracker.Utils.SystemWindowsWrappers;

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
        Mock<IDeckTextExport> mockDeckTextExportFormat;
        UserInfoMessages userInfoMessages;
        Mock<IClipboardWrapper> mockClipboardWrapper;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            mockDeckService = new Mock<IDeckService>();
            mockMessanger = new Mock<IMessenger>();
            mockfileSaver = new Mock<IFileSaver>();
            mockTracker = new Mock<ITracker>();
            mockProcessWrapper = new Mock<IProcessWrapper>();
            mockDeckTextExportFormat = new Mock<IDeckTextExport>();
            mockClipboardWrapper = new Mock<IClipboardWrapper>();
            userInfoMessages = new UserInfoMessages();
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
            


            mockMessanger.Verify(messanger =>
                       messanger.Send(
                           It.Is<EditDeck>(m => m.Deck == deck),
                           It.Is<EditDeck.Context>(c => c == EditDeck.Context.Hide)
                           ),
                       Times.Once,
                 "hide message not send ");
        }

        [TestMethod()]
        public void CommandUnHideDeckExecute()
        {
            Deck deck = new DeckBuilder().Build();

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.CommandUnHideDeckExecute(deck);

            Assert.AreEqual(false, deck.IsHidden);

            mockfileSaver.Verify(ms => ms.SaveDatabase(mockTracker.Object), Times.Once);

            mockMessanger.Verify(messanger =>
                 messanger.Send(
                     It.Is<EditDeck>(m => m.Deck == deck),
                     It.Is<EditDeck.Context>(c => c == EditDeck.Context.UnHide)
                     ),
                 Times.Once,
                 "unhide message not send ");
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

            mockMessanger.Verify(messanger =>
                 messanger.Send(
                     It.Is<EditDeck>(m => m.Deck == deck),
                     It.Is<EditDeck.Context>(c => c == EditDeck.Context.Delete)
                     ),
                 Times.Once,
                 "delete message not send ");

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

        [TestMethod]
        public void CommandExportToTextExecute()
        {
            List<CardInstance> samplecards = new List<CardInstance>()
                { new CardInstanceBuilder().WithQuantity(1).WithCard(CardsDatabase.FindCardByName("paarthurnax")).Build(),
                     new CardInstanceBuilder().WithQuantity(2).WithCard(CardsDatabase.FindCardByName("Miraak")).Build()
                };
            Deck deck = new DeckBuilder()
                .WithName("test")
                .WithSelectedVersion(
                    new DeckVersionBuilder().WithCards(samplecards).Build()
                ).Build();

            mockDeckService.Setup(ds => ds.CanExport(deck)).Returns(true);

            DeckItemMenuOperationsViewModel deckOps = CreateDeckItemMenuOperationsViewModelObject();

            deckOps.CommandExportToTextExecute(deck);

            mockDeckTextExportFormat.Verify(dtf  => dtf.FormatCardLine(It.Is<CardInstance>( ci=> ci.Quantity == 1)), Times.Once);
            mockDeckTextExportFormat.Verify(dtf  => dtf.FormatCardLine(It.Is<CardInstance>( ci=> ci.Quantity == 2)), Times.Once);
            mockDeckTextExportFormat.Verify(dtf  => dtf.FormatDeckHeader(It.IsAny<Deck>()), Times.Once);
            mockDeckTextExportFormat.Verify(dtf  => dtf.FormatDeckFooter(It.IsAny<Deck>()), Times.Once);
            mockClipboardWrapper.Verify(cw => cw.SetText(It.IsAny<string>()), Times.Once);

        }

        private DeckItemMenuOperationsViewModel CreateDeckItemMenuOperationsViewModelObject()
        {
            return new DeckItemMenuOperationsViewModel(
                mockMessanger.Object,
                mockfileSaver.Object,
                mockTracker.Object,
                mockDeckService.Object,
                mockProcessWrapper.Object,
                mockDeckTextExportFormat.Object,
                userInfoMessages,
                mockClipboardWrapper.Object);
        }
    }
}
