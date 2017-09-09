using ESLTracker.DataModel;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESLTracker.Services
{
    public class DeckImporter
    {
        public StringBuilder sbErrors { get; } = new StringBuilder();
        public string Status { get; set; }
        public List<CardInstance> Cards { get; set; }
        public bool DeltaImport { get; set; }

        private TaskCompletionSource<bool> taskCompletonSource;

        private ITrackerFactory trackerFactory;
        private ICardsDatabase cardsDatabase;

        public DeckImporter() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public DeckImporter(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            this.cardsDatabase = trackerFactory.GetService<ICardsDatabase>();
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
            string data = webClient.DownloadString(url);
            data = FindCardsData(data);
            if (!String.IsNullOrWhiteSpace(data))
            {
                Cards = new List<CardInstance>();
                sbErrors.Clear();

                try
                {
                    await Task.Run(() => ImportFromTextProcess(data));

                    taskCompletonSource?.SetResult(true);
                }
                catch (Exception ex)
                {
                    taskCompletonSource?.SetException(ex);
                }
            }

        }

        public string FindCardsData(string data)
        {
            var match = Regex.Match(data, $"<div.*id=('|\")bbModal('|\").*?<div.*?class.*?well_full.*?>(?<data>.*?)</div>", RegexOptions.Singleline);
            if (match.Success)
            {

                data = match.Groups["data"].Value;

                const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
                var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
                data = lineBreakRegex.Replace(data, Environment.NewLine);


                const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
                var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
                data = stripFormattingRegex.Replace(data, string.Empty);

                const string stripFormatting2 = @"\[[^]]*(\]|$)";//match any character between '<' and '>', even when end tag is missing
                var stripFormattingRegex2 = new Regex(stripFormatting2, RegexOptions.Multiline);
                data = stripFormattingRegex2.Replace(data, string.Empty);



            }
            else
            {
                data = String.Empty;
            }

            return data;
        }

        public void ImportFromTextProcess(string importData)
        {
            foreach (string cardLine in importData.Split(new string[] { Environment.NewLine },
                                                     StringSplitOptions.RemoveEmptyEntries))
            {
                if (String.IsNullOrWhiteSpace(cardLine))
                {
                    continue;
                }
                string[] splitedLine = cardLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                int cardCount = GetCardQty(splitedLine);
                string cardName = GetCardName(splitedLine);

                Card card = this.cardsDatabase.FindCardByName(cardName);

                CardInstance cardInstance = new CardInstance(card, trackerFactory);
                cardInstance.Quantity = cardCount;

                Cards.Add(cardInstance);
            }
        }

        internal void CancelImport()
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



        internal void ImportFinished(TaskCompletionSource<bool> tcs)
        {
            taskCompletonSource = tcs;
        }
    }
}
