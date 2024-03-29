﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TESLTracker.DataModel.Enums;
using TESLTracker.Utils;

namespace TESLTracker.DataModel
{
    public class Tracker : ViewModelBase, ITracker
    {

        public ObservableCollection<Game> Games { get; set; } = new ObservableCollection<Game>();
        public ObservableCollection<Deck> Decks { get; set; } = new ObservableCollection<Deck>();
        public ObservableCollection<Pack> Packs { get; set; } = new ObservableCollection<Pack>();

        public List<Reward> Rewards { get; set; } = new List<Reward>();

        public SerializableVersion Version { get; set; } = new SerializableVersion(0, 0);
  

        [XmlIgnore]
        public static SerializableVersion CurrentFileVersion = new SerializableVersion(3, 1);

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
                SetProperty<Deck>(ref activeDeck, value);
            }
        }

        [XmlIgnore]
        public IEnumerable<string> DeckTags
        {
            get
            {
                var tags = Games.Select(g => g.OpponentDeckTag).Distinct().Where(s => ! String.IsNullOrWhiteSpace(s));
                tags = tags.Union(Decks.Select(d => d.DeckTag)).Distinct().Where(s => !String.IsNullOrWhiteSpace(s));
                return tags.OrderBy(s => s).ToList();
            }
        }

        public IEnumerable<Reward> GetRewardsSummaryByType(RewardType type)
        {
            return Rewards.Where(r => r.Type == type);
        }

    }
}
