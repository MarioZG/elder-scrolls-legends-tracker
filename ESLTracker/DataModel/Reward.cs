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
                return new System.Windows.Media.Imaging.BitmapImage(new Uri(@"pack://application:,,,/"
                 + "Resources/RewardType/" + Type.ToString() + ".png"));
            }
        }
    }
}