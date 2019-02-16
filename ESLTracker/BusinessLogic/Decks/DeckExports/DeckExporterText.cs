using TESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.SystemWindowsWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Cards;

namespace ESLTracker.BusinessLogic.Decks.DeckExports
{
    public class DeckExporterText : IDeckExporter
    {
        private readonly IDeckTextExport deckTextExportFormat;
        private readonly UserInfoMessages userInfoMessages;
        private readonly IClipboardWrapper clipboardWrapper;
        private readonly ICardsDatabase cardsDatabase;
        private readonly ICardInstanceFactory cardsInstanceFactory;

        public DeckExporterText(
            IDeckTextExport deckTextExportFormat, 
            UserInfoMessages userInfoMessages, 
            IClipboardWrapper clipboardWrapper,
            ICardsDatabase cardsDatabase,
            ICardInstanceFactory cardsInstanceFactory)
        {
            this.deckTextExportFormat = deckTextExportFormat;
            this.userInfoMessages = userInfoMessages;
            this.clipboardWrapper = clipboardWrapper;
            this.cardsDatabase = cardsDatabase;
            this.cardsInstanceFactory = cardsInstanceFactory;
        }

        public bool ExportDeck(Deck deck)
        {
            if (deck != null)
            {
                var cards = deck.SelectedVersion.Cards
                    .Select(c => c.Card.DoubleCard.HasValue ? cardsInstanceFactory.CreateFromCard(cardsDatabase.FindCardById(c.Card.DoubleCard.Value), c.Quantity) : c)
                    .Select(c => c.Card.DoubleCardComponents != null ? cardsInstanceFactory.CreateFromCard(c.Card, c.Quantity / 2) : c).OrderBy(c => c?.Card?.Cost)
                    .ThenBy(c => c?.Card?.Name)
                    .Select(c => this.deckTextExportFormat.FormatCardLine(c)).ToList();

                cards.Insert(0, this.deckTextExportFormat.FormatDeckHeader(deck));
                cards.Add(this.deckTextExportFormat.FormatDeckFooter(deck));

                clipboardWrapper.SetText(string.Join(Environment.NewLine, cards));

                userInfoMessages.AddMessage($" {deck.Name} content has been exported as BB text to clipboard. Ctrl+v to paste.");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
