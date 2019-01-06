using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Decks.DeckImports;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.SystemWindowsWrappers;

namespace ESLTracker.BusinessLogic.Decks.DeckExports
{
    public class DeckExporterSPCode : IDeckExporter
    {
        private readonly ICardSPCodeProvider cardSPCodeProvider;
        private readonly UserInfoMessages userInfoMessages;
        private readonly IClipboardWrapper clipboardWrapper;

        public DeckExporterSPCode(
            ICardSPCodeProvider cardSPCodeProvider,
            UserInfoMessages userInfoMessages,
            IClipboardWrapper clipboardWrapper)
        {
            this.cardSPCodeProvider = cardSPCodeProvider;
            this.userInfoMessages = userInfoMessages;
            this.clipboardWrapper = clipboardWrapper;
        }

        public bool ExportDeck(Deck deck)
        {
            if (deck != null)
            {
                var codeChars = deck.SelectedVersion.Cards
                            .OrderBy(c => c?.Card?.Cost)
                            .ThenBy(c => c?.Card?.Name)
                            .Union(new[] {
                                new CardInstance() { Quantity = 1, Card = Card.Unknown },
                                new CardInstance() { Quantity = 2, Card = Card.Unknown },
                                new CardInstance() { Quantity = 3, Card = Card.Unknown }
                            })
                            .Select(c => new { Qty = c.Quantity, Code = TranslateCardNameToCode(c?.Card?.Name) })
                            .OrderBy(g => g.Qty)
                            .GroupBy(c => c.Qty)
                            .Select(g => (new string[] { QtyToCode(g.Count()) }).Union(g.Select(c => c.Code)))
                            .SelectMany(c => c)
                            .ToArray();

                var code = $"SP{String.Join("", codeChars)}";

                clipboardWrapper.SetText(string.Join(Environment.NewLine, code));

                userInfoMessages.AddMessage($" {deck.Name} content has been exported as SP code to clipboard. Create new deck in game to use it!");
                return true;
            }
            else
            {
                return false;
            }
        }

        private string TranslateCardNameToCode(string cardName)
        {
            if (cardName == Card.Unknown.Name)
            {
                return String.Empty;
            }
            else
            {
                return cardSPCodeProvider.GetCodeByCardName(cardName);
            }
        }

        private string QtyToCode(int qty)
        {
            return $"{(char)(qty / 26 + 65)}{(char)(qty % 26 + 64)}";
        }
        
    }
}
