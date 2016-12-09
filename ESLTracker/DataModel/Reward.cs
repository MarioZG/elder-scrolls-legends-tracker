using System;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.DataModel
{
    public class Reward
    {
        public Reward() : this(TrackerFactory.DefaultTrackerFactory)
        {
        }

        internal Reward(ITrackerFactory factory)
        {
            Date = factory.GetDateTimeNow();
        }

        public RewardReason Reason { get; set; }
        public RewardType Type { get; set; }
        public int Quantity { get; set; }
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

        [XmlIgnore]
        public System.Windows.Media.ImageSource TypeIcon
        {
            get
            {
                try
                {
                    return new System.Windows.Media.Imaging.BitmapImage(new Uri(@"pack://application:,,,/"
                     + "Resources/RewardType/" + Type.ToString() + ".png"));
                }
                catch (System.IO.IOException)
                {
                    return null;
                }
            }
        
        }
    }
}