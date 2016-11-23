using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.Utils;

namespace ESLTracker.DataModel
{
    public class Tracker : INotifyPropertyChanged
    {
        private static Tracker _instance = null;
        public static Tracker Instance
        {
            get
            {
                if(_instance == null)
                {
                    try
                    {
                        _instance = Utils.FileManager.LoadDatabase();
                        //fix up ref to decks in games
                        foreach (Game g in _instance.Games)
                        {
                            g.Deck = _instance.Decks.Where(d => d.DeckId == g.DeckId).FirstOrDefault();
                        }
                        //fix up ref to decks in rewards
                        foreach (Reward r in _instance.Rewards)
                        {
                            r.ArenaDeck = _instance.Decks.Where(d => d.DeckId == r.ArenaDeckId).FirstOrDefault();
                        }
                        //restore active deck
                        Guid? activeDeckFromSettings = Settings.Default.LastActiveDeckId;
                        if ((activeDeckFromSettings != null)
                            && (activeDeckFromSettings != Guid.Empty))
                        {
                            _instance.ActiveDeck = _instance.Decks.Where(d => d.DeckId == activeDeckFromSettings).FirstOrDefault();
                        }
                    }
                    catch
                    {
                        _instance = new Tracker();
                    }                   
                }
                return _instance;
            }
        }

        public ObservableCollection<Game> Games = new ObservableCollection<Game>();
        public ObservableCollection<Deck> Decks = new ObservableCollection<Deck>();

        public List<Reward> Rewards = new List<Reward>();

        public SerializableVersion Version = new SerializableVersion(1,0);
        
        // binding!!!

        //Deck selected in applications
        private Deck activeDeck;
        [XmlIgnore]
        public Deck ActiveDeck
        {
            get
            {
                return activeDeck;
            }
            set
            {
                activeDeck = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ActiveDeck")); 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Reward> GetRewardsSummaryByType(RewardType type)
        {
            return Instance.Rewards.Where(r => r.Type == type);
        }

    }
}
