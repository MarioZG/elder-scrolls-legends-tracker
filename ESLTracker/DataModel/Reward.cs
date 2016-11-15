using System;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.DataModel
{
    public class Reward
    {
        public Reward()
        {
            Date = DateTime.Now;
        }

        public RewardReason Reason { get; set; }
        public RewardType Type { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; }

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
    }
}