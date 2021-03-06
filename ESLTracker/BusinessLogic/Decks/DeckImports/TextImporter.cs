﻿using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using TESLTracker.DataModel;
using ESLTracker.Utils;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Decks.DeckImports
{
    public class TextImporter : BaseImporter
    {
        private readonly IDeckService deckService;

        public bool DeltaImport { get; set; }


        public TextImporter(
            ICardsDatabaseFactory cardsDatabaseFactory, 
            ICardInstanceFactory cardInstanceFactory, 
            IDeckService deckService,
            DeckCardsEditor deckCardsEditor)
                : base(cardsDatabaseFactory, cardInstanceFactory, deckCardsEditor)
        {
            this.deckService = deckService;
        }

        public override bool ValidateInput(object data)
        {
            return data != null;
        }

        override internal void ExecuteImport(object importData)
        {
            if (! (importData is string))
            {
                throw new ArgumentException("Text importer expects string as paramater");
            }

            var importLines = importData.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            int titleLine = 0;
            if (importLines[0].StartsWith("###"))
            {
                DeckName = importLines[0].Replace("###", String.Empty).Trim();
                titleLine = 1;
            }

            foreach (string cardLine in importLines.Skip(titleLine))
            {
                if (String.IsNullOrWhiteSpace(cardLine))
                {
                    continue;
                }
                var cardData = RemoveTextInCurlyBraces(cardLine);
                string[] splitedLine = cardData.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                int cardCount = GetCardQty(splitedLine);
                string cardName = GetCardName(splitedLine);

                deckCardsEditor.ChangeCardQuantity(Cards, cardName, cardCount, false);
            }
        }

        internal override void ExecutePostImportFixUp(Deck deck)
        {
            base.ExecutePostImportFixUp(deck);
            if (DeltaImport)
            {
                foreach (var importedCard in Cards)
                {
                    deckCardsEditor.ChangeCardQuantity(
                        deck.SelectedVersion.Cards,
                        importedCard.Card,
                        importedCard.Quantity,
                        deckService.LimitCardCountForDeck(deck));
                }
            }
        }

        internal string GetCardName(string[] cardLine)
        {
            return String.Join(" ", cardLine.Skip(1));
        }

        internal int GetCardQty(string[] cardLine)
        {
            int value;
            if (Int32.TryParse(cardLine[0], out value))
            {
                return value;
            }
            else
            {
                this.Errors.AppendLine(String.Join(" ", cardLine) + ": cannot parse quantity");
                return 0;
            }
        }

        internal  string RemoveTextInCurlyBraces(string data)
        {
            const string stripFormatting2 = @"\[[^\]]*(\]|$)";//match any character between '[' and ']', even when end tag is missing
            var stripFormattingRegex2 = new Regex(stripFormatting2, RegexOptions.Multiline);
            data = stripFormattingRegex2.Replace(data, string.Empty);
            return data;
        }
    }
}
