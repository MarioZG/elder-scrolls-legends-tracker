using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Cards;
using Newtonsoft.Json;

namespace ESLTracker.BusinessLogic.Decks.DeckImports
{
    public class SPCodeImporter : BaseImporter
    {
        private const string CodePrefix = "SP";

        private readonly ICardSPCodeProvider cardSPCodeProvider;

        public SPCodeImporter(
            ICardsDatabaseFactory cardsDatabaseFactory, 
            ICardInstanceFactory cardInstanceFactory,
            ICardSPCodeProvider cardSPCodeProvider,
            DeckCardsEditor deckCardsEditor) 
            : base(cardsDatabaseFactory, cardInstanceFactory, deckCardsEditor)
        {
            this.cardSPCodeProvider = cardSPCodeProvider;
        }

        internal override void ExecuteImport(object importData)
        {
            if (!(importData is string) || !ValidateInput(importData.ToString().Trim()))
            {
                throw new ArgumentException("Text importer expects SP string");
            }

            ICardsDatabase cardsDatabase = cardsDatabaseFactory.GetCardsDatabase();

            try
            {
                var code = importData.ToString().Trim().Substring(2);


                for (int qty = 1; qty <= 3; qty++)
                {
                    int noOfCards = CodeToQty(code.Take(2).ToArray());
                    for (int cardNo = 0; cardNo < noOfCards; cardNo++)
                    {
                        try
                        {
                            var name = cardSPCodeProvider.GetCardByCode(code.Substring(2 + cardNo * 2, 2));
                            deckCardsEditor.ChangeCardQuantity(Cards, name, qty, false);
                        }
                        catch (MissingSPCodeException ex)
                        {
                            Errors.AppendLine(ex.Message);
                        }
                    }
                    code = code.Substring(2 + noOfCards * 2);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Invalid SP code. ({ex.Message})", ex);
            }

        }

        public override bool ValidateInput(object data)
        {
            return data != null && data.ToString().StartsWith(CodePrefix);
        }

        private int CodeToQty(char[] code)
        {
            return (((int)code[0] - 65) * 26 + ((int)code[1] - 65));
        }
    }
}
