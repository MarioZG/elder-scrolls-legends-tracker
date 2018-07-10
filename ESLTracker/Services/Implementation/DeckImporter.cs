using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESLTracker.Services
{
    public class DeckImporter : IDeckImporter
    {
        public StringBuilder sbErrors { get; } = new StringBuilder();
        public string Status { get; set; }
        public List<CardInstance> Cards { get; set; }
        public bool DeltaImport { get; set; }
        public string DeckName { get; private set; }

        private TaskCompletionSource<bool> taskCompletonSource;

        private readonly ICardsDatabase cardsDatabase;
        private readonly ICardInstanceFactory cardInstanceFactory;

        public DeckImporter(ICardsDatabase cardsDatabase, ICardInstanceFactory cardInstanceFactory)
        {
            this.cardsDatabase = cardsDatabase;
            this.cardInstanceFactory = cardInstanceFactory;
        }

        public async Task ImportFromText(string importData)
        {
            Cards = new List<CardInstance>();
            sbErrors.Clear();

            try
            {
                await Task.Run(() => ImportFromTextProcess(importData));

                taskCompletonSource.SetResult(true);
            }
            catch (Exception ex)
            {
                taskCompletonSource.SetException(ex);
            }
        }

        public async Task ImportFromWeb(string url)
        {
            WebClient webClient = new WebClient();
            var data = await Task.Run(() => webClient.DownloadString(url));
            data = FindCardsData(data);
            if (!String.IsNullOrWhiteSpace(data))
            {
                Cards = new List<CardInstance>();
                sbErrors.Clear();

                try
                {
                    ImportFromTextProcess(data);

                    taskCompletonSource?.SetResult(true);
                }
                catch (Exception ex)
                {
                    taskCompletonSource?.SetResult(false);
                    taskCompletonSource?.SetException(ex);
                }
            }
            else
            {
                sbErrors.AppendLine("No cards found on page.");
                taskCompletonSource?.SetResult(false);
            }

        }

        public string FindCardsData(string data)
        {
            //var match = Regex.Match(data, $"<div.*id=('|\")bbModal('|\").*?<div.*?class.*?well_full.*?>(?<data>.*?)</div>", RegexOptions.Singleline);
            var match = Regex.Match(data, $"<div class=\"well_full\" id=\"clipcopy\">(?<data>.*?)</div>", RegexOptions.Singleline);
            if (match.Success)
            {

                data = match.Groups["data"].Value;

                const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
                var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
                data = lineBreakRegex.Replace(data, Environment.NewLine);

            }
            else
            {
                data = String.Empty;
            }

            return data;
        }

        private static string RemoveTextInCurlyBraces(string data)
        {
            const string stripFormatting2 = @"\([^)]*(\)|$)";//match any character between '(' and ')', even when end tag is missing
            var stripFormattingRegex2 = new Regex(stripFormatting2, RegexOptions.Multiline);
            data = stripFormattingRegex2.Replace(data, string.Empty);
            return data;
        }

        public void ImportFromTextProcess(string importData)
        {
            var importLines = importData.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);

            this.DeckName = importLines[0].Replace("###", String.Empty).Trim();

            foreach (string cardLine in importLines.Skip(1))
            {
                if (String.IsNullOrWhiteSpace(cardLine))
                {
                    continue;
                }
                var cardData = RemoveTextInCurlyBraces(cardLine);
                string[] splitedLine = cardData.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                int cardCount = GetCardQty(splitedLine);
                string cardName = GetCardName(splitedLine);

                Card card = this.cardsDatabase.FindCardByName(cardName);

                CardInstance cardInstance = cardInstanceFactory.CreateFromCard(card, cardCount);

                Cards.Add(cardInstance);
            }
        }

        public void CancelImport()
        {
            taskCompletonSource.TrySetResult(false);
        }

        public string GetCardName(string[] cardLine)
        {
            return String.Join(" ", cardLine.Skip(1));
        }

        public int GetCardQty(string[] cardLine)
        {
            int value;
            if (Int32.TryParse(cardLine[0], out value))
            {
                return value;
            }
            else
            {
                this.sbErrors.AppendLine(String.Join(" ", cardLine) + ": cannot parse quantity");
                return 0;
            }
        }



        public void ImportFinished(TaskCompletionSource<bool> tcs)
        {
            taskCompletonSource = tcs;
        }
    }
}
