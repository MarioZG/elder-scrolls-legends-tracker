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
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels;


namespace ESLTracker.DataModel
{
    [DebuggerDisplay("{DebuggerInfo}")]
    public class CardInstance : ViewModelBase, ICloneable
    {
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
            set { SetProperty(ref isPremium, value, onChanged: () => { RaisePropertyChangedEvent(String.Empty); }); }
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

        private void LoadCardFromDataBase(Guid value)
        {
            this.Card = MasserContainer.Container.GetInstance<ICardsDatabaseFactory>().GetCardsDatabase().FindCardById(value);
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
