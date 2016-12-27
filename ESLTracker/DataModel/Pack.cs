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
        public ObservableCollection<CardInstance> Cards { get; set; } = new ObservableCollection<CardInstance>()
             { new CardInstance(), new CardInstance(), new CardInstance(), new CardInstance(), new CardInstance(), new CardInstance()};

        public DateTime DateOpened { get; set; }

        public int SoulGemsValue
        {
            get
            {
                var notNullCards = Cards.Where(c => c.Card != null && c.Card != Card.Unknown);
                return
                    notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Common && ! c.IsGolden).Count() * 5
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Common && c.IsGolden).Count() * 50
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Rare && !c.IsGolden).Count() * 20
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Rare && c.IsGolden).Count() * 100
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Epic && !c.IsGolden).Count() * 100
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Epic && c.IsGolden).Count() * 400
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Legendary && !c.IsGolden).Count() * 400
                    + notNullCards.Where(c => c.Card.Rarity == Enums.CardRarity.Legendary && c.IsGolden).Count() * 1200;
            }
        }

        public Pack()
        {
            Cards.CollectionChanged += Cards_CollectionChanged;
        }

        private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChangedEvent(nameof(SoulGemsValue));
        }
    }
}
