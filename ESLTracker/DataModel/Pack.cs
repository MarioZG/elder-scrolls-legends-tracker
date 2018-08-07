using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.Utils;
using ESLTracker.ViewModels;

namespace ESLTracker.DataModel
{
    public class Pack : ViewModelBase
    {
        private SoulGemCalculator soulGemCalculator = new SoulGemCalculator();

        public ObservableCollection<CardInstance> Cards { get; set; }

        public DateTime DateOpened { get; set; }

        private CardSet cardSet;
        public CardSet CardSet
        {
            get { return cardSet; }
            set { SetProperty<CardSet>(ref cardSet, value); }
        }
        public int SoulGemsValue
        {
            get
            {
                return soulGemCalculator.CalculateCardsSellValue(Cards);
            }
        }

        [Obsolete("Use factory in production code or deckbuilder in unit tests to create new packs")]
        public Pack()
        {
           
        }

        internal void RefreshBindings(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
