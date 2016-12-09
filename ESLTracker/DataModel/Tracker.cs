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
    public class Tracker : INotifyPropertyChanged, ITracker
    {
        private static Tracker _instance = null;
        public static Tracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FileManager(TrackerFactory.DefaultTrackerFactory).LoadDatabase();
                }
                return _instance;
            }
        }

        public ObservableCollection<Game> Games { get; set; } = new ObservableCollection<Game>();
        public ObservableCollection<Deck> Decks { get; set; } = new ObservableCollection<Deck>();

        public List<Reward> Rewards { get; set; } = new List<Reward>();

        public SerializableVersion Version { get; set; } = new SerializableVersion(0, 0);
  

        [XmlIgnore]
        public static SerializableVersion CurrentFileVersion = new SerializableVersion(1, 1);

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
