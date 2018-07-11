using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.Utils.Messages;
using System;
using System.Linq;

namespace ESLTracker.BusinessLogic.General
{
    public class TrackerFactory : ITrackerFactory
    {
        IMessenger messenger;
        FileLoader fileLoader;
        ISettings settings;

        public TrackerFactory(IMessenger messenger, FileLoader fileLoader, ISettings settings)
        {
            this.messenger = messenger;
            this.fileLoader = fileLoader;
            this.settings = settings;
        }

        public static object _instanceLock = new object();
        public static Tracker _instance = null;

        public ITracker GetTrackerInstance()
        {
            lock (_instanceLock)
            {
                if (_instance == null)
                {
                    _instance = fileLoader.LoadDatabase();
                    FixUpDeserializedTracker(_instance);
                    SetActiveDeck(_instance);
                }
                return _instance;
            }
        }

        private void SetActiveDeck(ITracker tracker)
        {
            //restore active deck
            Guid? activeDeckFromSettings = settings.LastActiveDeckId;
            if ((activeDeckFromSettings != null)
                && (activeDeckFromSettings != Guid.Empty))
            {
                tracker.ActiveDeck = tracker.Decks.Where(d => d.DeckId == activeDeckFromSettings).FirstOrDefault();
            }
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
