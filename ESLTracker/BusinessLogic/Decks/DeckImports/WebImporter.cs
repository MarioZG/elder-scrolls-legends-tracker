using ESLTracker.BusinessLogic.Cards;
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
    public class WebImporter : TextImporter
    {
        public WebImporter(ICardsDatabaseFactory cardsDatabaseFactory, ICardInstanceFactory cardInstanceFactory)
            :base(cardsDatabaseFactory, cardInstanceFactory, null)
        {

        }

        override internal void ExecuteImport(object importData)
        {
            WebClient webClient = new WebClient();
            var data = webClient.DownloadString(importData.ToString());
            data = FindCardsData(data);

            base.ExecuteImport(data);

        }

        internal override void ExecutePostImportFixUp(Deck deck)
        {
            deck.DeckUrl = ImportData?.ToString();

            base.ExecutePostImportFixUp(deck);
        }

        public override bool ValidateInput(object data)
        {
            var url = data.ToString().Trim().ToLower();
            return url.StartsWith("https://") || url.StartsWith("http://");
        }

        internal string FindCardsData(string data)
        {
            string cardsData = String.Empty;
            var match = Regex.Match(data, $"<div class=\"well_full\">(?<data>.*?)</div>", RegexOptions.Singleline);
            if (match.Success)
            {

                cardsData = match.Groups["data"].Value;

                const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
                var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
                cardsData = lineBreakRegex.Replace(cardsData, Environment.NewLine);

                var titleMatch = Regex.Match(data, $"<li class=\"active\">(?<title>.*?)</li>", RegexOptions.Singleline);
                if (titleMatch.Success)
                {
                    cardsData = $"###{titleMatch.Groups["title"].Value}###{Environment.NewLine}" + cardsData;
                }
            }

            return cardsData;
        }    
    }
}
