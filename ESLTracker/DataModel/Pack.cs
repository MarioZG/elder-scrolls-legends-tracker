using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils;
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
                return SoulGemsHelper.CalculateCardsSellValue(Cards);
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
