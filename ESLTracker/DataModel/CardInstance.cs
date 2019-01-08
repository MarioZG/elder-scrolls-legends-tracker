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
using ESLTracker.ViewModels;


namespace ESLTracker.DataModel
{
    [DebuggerDisplay("{DebuggerInfo}")]
    public class CardInstance : ViewModelBase, ICloneable
    {
        private Guid cardId;
        public Guid CardId
        {
            get
            {
                return Card?.Id ?? cardId;

            }
            set
            {
                cardId = value;
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
            set { SetProperty(ref isPremium, value, onChanged: () => { RaisePropertyChangedEvent(nameof(IsPremium)); }); }
        }

        private int quantity = 1;

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; RaisePropertyChangedEvent(nameof(Quantity)); }
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

        public CardInstance()
        {

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
