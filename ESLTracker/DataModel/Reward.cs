using System;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.ViewModels;

namespace ESLTracker.DataModel
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
                RaisePropertyChangedEvent(nameof(TypeIcon));
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

        [Obsolete("Use factory in production code or deckbuilder in unit tests to create new decks")]
        public Reward()
        {
       
        }
    }
}