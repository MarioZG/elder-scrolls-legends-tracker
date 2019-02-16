using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.BusinessLogic.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTrackerTests.Builders;
using TESLTracker.DataModel;
using Moq;
using ESLTracker.Utils;
using ESLTracker.Utils.SystemWindowsWrappers;
using ESLTracker.BusinessLogic.Decks.DeckExports;

namespace ESLTracker.BusinessLogic.Decks.DeckExports.Tests
{
    [TestClass()]
    public class DeckExporterTextTests 
    {
        Mock<IDeckTextExport> mockDeckTextExportFormat;
        UserInfoMessages userInfoMessages;
        Mock<IClipboardWrapper> mockClipboardWrapper;

        [TestInitialize]
        public void TestInitialize()
        {
            mockDeckTextExportFormat = new Mock<IDeckTextExport>();
            mockClipboardWrapper = new Mock<IClipboardWrapper>();
            userInfoMessages = new UserInfoMessages();
        }

        [TestMethod()]
        public void ExportDeckTest()
        {
            Card card1 = new Card() { Name = "card 1" };
            Card card2 = new Card() { Name = "some other card" };

            int qty1 = 1;
            int qty2 = 3;

            List<CardInstance> samplecards = new List<CardInstance>()
                { new CardInstanceBuilder().WithQuantity(qty1).WithCard(card1).Build(),
                     new CardInstanceBuilder().WithQuantity(qty2).WithCard(card2).Build()
                };
            Deck deck = new DeckBuilder()
                .WithName("test")
                .WithSelectedVersion(
                    new DeckVersionBuilder().WithCards(samplecards).Build()
                ).Build();

            DeckExporterText deckOps = CreateDeckExporterTextObject();

            deckOps.ExportDeck(deck);

            mockDeckTextExportFormat.Verify(dtf => dtf.FormatCardLine(It.Is<CardInstance>(ci => ci.Quantity == qty1 && ci.Card.Name == card1.Name)), Times.Once);
            mockDeckTextExportFormat.Verify(dtf => dtf.FormatCardLine(It.Is<CardInstance>(ci => ci.Quantity == qty2 && ci.Card.Name == card2.Name)), Times.Once);
            mockDeckTextExportFormat.Verify(dtf => dtf.FormatDeckHeader(It.IsAny<Deck>()), Times.Once);
            mockDeckTextExportFormat.Verify(dtf => dtf.FormatDeckFooter(It.IsAny<Deck>()), Times.Once);
            mockClipboardWrapper.Verify(cw => cw.SetText(It.IsAny<string>()), Times.Once);
        }

        private DeckExporterText CreateDeckExporterTextObject()
        {
            return new DeckExporterText(
                mockDeckTextExportFormat.Object,
                userInfoMessages,
                mockClipboardWrapper.Object,
                null, //used only for double cards cases
                null);//used only for double cards cases
        }
    }
}