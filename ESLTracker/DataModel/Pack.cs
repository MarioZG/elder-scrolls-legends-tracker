using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.ViewModels;

namespace ESLTracker.DataModel
{
    public class Pack : ViewModelBase
    {
        public ObservableCollection<CardInstance> Cards { get; set; }

        public DateTime DateOpened { get; set; }

        public int SoulGemsValue
        {
            get
            {
                var notNullCards = Cards.Where(c => c.Card != null && c.Card != Card.Unknown);
                return
                    notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Common && ! c.IsPremium).Count() * 5
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Common && c.IsPremium).Count() * 50
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Rare && !c.IsPremium).Count() * 20
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Rare && c.IsPremium).Count() * 100
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Epic && !c.IsPremium).Count() * 100
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Epic && c.IsPremium).Count() * 400
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Legendary && !c.IsPremium).Count() * 400
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Legendary && c.IsPremium).Count() * 1200;
            }
        }

        public Pack() : this(null, false)
        {
           
        }

        public Pack(IEnumerable<CardInstance> startringCards, bool raiseChangeEventsOnCards)
        {
            if (startringCards == null)
            {
                Cards = new ObservableCollection<CardInstance>();
            }
            else
            {
                Cards = new ObservableCollection<CardInstance>(startringCards);
            }

            if (raiseChangeEventsOnCards)
            {
                Cards.CollectionChanged += Cards_CollectionChanged;
            }

            if ((startringCards != null) && raiseChangeEventsOnCards)
            {
                Cards.All(c => { c.PropertyChanged += CardInstance_PropertyChanged; return true; });

            }
        }

        private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CardInstance ci in e.NewItems)
                {
                    ci.PropertyChanged += CardInstance_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (CardInstance ci in e.OldItems)
                {
                    ci.PropertyChanged -= CardInstance_PropertyChanged;
                }
            }
        }

        private void CardInstance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.PropertyName)
                || (e.PropertyName == nameof(CardInstance.Card))
                || (e.PropertyName == nameof(CardInstance.IsPremium)))
            {
                RaisePropertyChangedEvent(nameof(SoulGemsValue));
            }
        }
    }
}
