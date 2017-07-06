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
using ESLTracker.Services;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.ViewModels;


namespace ESLTracker.DataModel
{
    [DebuggerDisplay("{DebuggerInfo}")]
    public class CardInstance : ViewModelBase, ICloneable
    {
        private ITrackerFactory trackerFactory;
        private static ICardImageService cardImageService;
        private static ICardsDatabase cardsDatabase;

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

        private int quantity = 1;

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; RaisePropertyChangedEvent(nameof(Quantity)); }
        }

        [XmlIgnore]
        public Brush BackgroundColor
        {
            get
            {
                return cardImageService.GetCardMiniature(card);
            }
        }
        [XmlIgnore]
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
        [XmlIgnore]
        public Brush RarityColor
        {
            get
            {
                return cardImageService.GetRarityBrush(card);
            }
        }

        private Brush borderBrush = null;
        [XmlIgnore]
        public Brush BorderBrush
        {
            get { return borderBrush; }
            set { borderBrush = value; RaisePropertyChangedEvent(nameof(BorderBrush)); }
        }

        public bool HasCard
        {
            get
            {
                return ((card != null) && (card != Card.Unknown));
            }
        }

        public string DebuggerInfo
        {
            get
            {
                return string.Format("Card={0};IsPremium={1};Qty={2}", Card?.Name, IsPremium, Quantity);
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
            if (cardImageService == null)
            {
                cardImageService = trackerFactory.GetService<ICardImageService>();
            }
            if (cardsDatabase == null)
            {
                cardsDatabase = trackerFactory.GetService<ICardsDatabase>();
            }
        }

        private void LoadCardFromDataBase(Guid value)
        {
            this.Card = cardsDatabase.FindCardById(value);
        }

        public object Clone()
        {
            CardInstance ci =  this.MemberwiseClone() as CardInstance;
            ci.ClearPropertyChanged();
            //if (ci != null)
            //{
               
            //}
            return ci;
        }
    }
}
