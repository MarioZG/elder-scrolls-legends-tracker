using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Services;

namespace ESLTracker.Utils
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

        private void ImportFromTextProcess(string importData)
        {
            foreach (string cardLine in importData.Split(new string[] { Environment.NewLine },
                                                     StringSplitOptions.RemoveEmptyEntries))
            {
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
