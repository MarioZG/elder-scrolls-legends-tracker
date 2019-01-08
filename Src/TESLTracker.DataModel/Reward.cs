using System;
using System.Xml.Serialization;
using TESLTracker.DataModel.Enums;
using TESLTracker.Utils;

namespace TESLTracker.DataModel
{
    public class Reward : ViewModelBase
    {
        public RewardReason Reason { get; set; }

        private RewardType type;
        public RewardType Type
        {
            get { return type; }
            set {
                SetProperty(ref type, value);
            }
        }

        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity, value); }
        }

        public string Comment { get; set; }
        public DateTime Date { get; set; }

        //quest rewards
        public Guild? RewardQuestGuild {get;set;}

        //arena reward
        [XmlIgnore]
        public Deck ArenaDeck { get; set; }

        private Guid? arenaDeckId = null;
        public Guid? ArenaDeckId
        {
            get
            {
                return (ArenaDeck != null) ? ArenaDeck.DeckId : arenaDeckId;
            }
            set
            {
                arenaDeckId = value;
            }
        }

        public CardInstance CardInstance { get; set; }

        [Obsolete("Use factory in production code or deckbuilder in unit tests to create new decks")]
        public Reward()
        {
       
        }
    }
}