using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.SystemWindowsWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Decks
{
    public class DeckExporterText : IDeckExporterText
    {
        private readonly IDeckTextExport deckTextExportFormat;
        private readonly UserInfoMessages userInfoMessages;
        private readonly IClipboardWrapper clipboardWrapper;

        public DeckExporterText(
            IDeckTextExport deckTextExportFormat, 
            UserInfoMessages userInfoMessages, 
            IClipboardWrapper clipboardWrapper)
        {
            this.deckTextExportFormat = deckTextExportFormat;
            this.userInfoMessages = userInfoMessages;
            this.clipboardWrapper = clipboardWrapper;
        }

        public bool ExportDeck(Deck deck)
        {
            if (deck != null)
            {
                var cards = deck.SelectedVersion.Cards
                    .OrderBy(c => c?.Card?.Cost)
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
