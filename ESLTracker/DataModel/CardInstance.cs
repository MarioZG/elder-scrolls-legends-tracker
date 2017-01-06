using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.ViewModels;


namespace ESLTracker.DataModel
{
    [DebuggerDisplay("Card={Card.Name}; Premium={IsPremium}")]
    public class CardInstance : ViewModelBase
    {
        private ITrackerFactory trackerFactory;

        public Guid CardId
        {
            get
            {
                return Card != null ? Card.Id : Card.Unknown.Id;
            }
            set
            {
                LoadCardFromDataBase(value);
            }
        }

        Card card;
        [XmlIgnore]
        public Card Card
        {
            get { return card; }
            set { card = value; RaisePropertyChangedEvent(String.Empty); }
        }
        private bool isPremium;

        public bool IsPremium
        {
            get { return isPremium; }
            set { isPremium = value;  RaisePropertyChangedEvent(nameof(IsPremium)); }
        }

        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; RaisePropertyChangedEvent(nameof(Quantity)); }
        }


        public Brush BackgroundColor
        {
            get
            {
                return CardImagesHelper.GetCardMiniature(card);
            }
        }

        public Brush ForegroundColor
        {
            get
            {
                if ((Card != null) && (Card != Card.Unknown))
                {
                    return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                }
            }
        }

        public Brush RarityColor
        {
            get
            {
                return CardImagesHelper.GetRarityBrush(card);
            }
        }       


        public bool HasCard
        {
            get
            {
                return ((card != null) && (card != Card.Unknown));
            }
        }
        public CardInstance() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public CardInstance(Card card) : this(card, TrackerFactory.DefaultTrackerFactory)
        {

        }

        public CardInstance(ITrackerFactory trackerFactory) : this(null, trackerFactory)
        {
            
        }

        public CardInstance(Card card, ITrackerFactory trackerFactory)
        {
            this.Card = card;
            this.trackerFactory = trackerFactory;
        }

        private void LoadCardFromDataBase(Guid value)
        {
            this.Card = trackerFactory.GetCardsDatabase().FindCardById(value);
        }
    }
}
