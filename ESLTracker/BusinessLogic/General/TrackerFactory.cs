using ESLTracker.DataModel;
using ESLTracker.Services;
using ESLTracker.Utils.Messages;
using System.Linq;

namespace ESLTracker.BusinessLogic.General
{
    public class TrackerFactory : ITrackerFactory
    {
        IMessenger messenger;

        public TrackerFactory(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        public ITracker CreateEmptyTracker()
        {
            var tracker = new Tracker();
            tracker.Version = Tracker.CurrentFileVersion;
            tracker.PropertyChanged += ActiveDeckChanged;
            return tracker;
        }

        public void FixUpDeserializedTracker(ITracker tracker)
        {
            //fix up ref to decks in games
            foreach (Game g in tracker.Games)
            {
                g.Deck = tracker.Decks.Where(d => d.DeckId == g.DeckId).FirstOrDefault();
            }

            //fix up ref to decks in rewards
            foreach (Reward r in tracker.Rewards)
            {
                r.ArenaDeck = tracker.Decks.Where(d => d.DeckId == r.ArenaDeckId).FirstOrDefault();
            }

            //hook up active deck changed event
            tracker.PropertyChanged += ActiveDeckChanged;
        }

        private void ActiveDeckChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Tracker.ActiveDeck))
            {
                messenger.Send(new ActiveDeckChanged(((Tracker)sender).ActiveDeck));
            }
        }
    }
}
