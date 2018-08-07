using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Packs
{
    public class PackFactory
    {

        public const int PackSize = 6;
        IEnumerable<CardInstance> CardsForEmptyPack => Enumerable.Range(1, PackSize).Select(i => new CardInstance());

        public Pack CreatePack(IEnumerable<CardInstance> startringCards)
        {
            return CreatePack(startringCards, false);
        }

        public Pack CreatePack(IEnumerable<CardInstance> startringCards, bool raiseChangeEventsOnCards)
        {
            #pragma warning disable CS0618 // used in factory
            Pack pack = new Pack();
            #pragma warning restore CS0618 // used in factory
            if (startringCards == null)
            {
                pack.Cards = new ObservableCollection<CardInstance>();
            }
            else
            {
                pack.Cards = new ObservableCollection<CardInstance>(startringCards);
            }

            if (raiseChangeEventsOnCards)
            {
                SetUpChangeEvents(pack);
            }

            return pack;
        }

        public Pack CreateEmptyPack(bool raiseChangeEventsOnCards)
        {
            return CreatePack(
                CardsForEmptyPack,
                raiseChangeEventsOnCards);
        }

        public void ClearPack(Pack pack)
        {
            pack.Cards = new ObservableCollection<CardInstance>(CardsForEmptyPack);
            SetUpChangeEvents(pack);
        }

        private void SetUpChangeEvents(Pack pack)
        {
            pack.Cards.CollectionChanged += Cards_CollectionChanged;

            if (pack.Cards != null)
            {
                pack.Cards.All(c => { c.PropertyChanged += pack.RefreshBindings; return true; });
            }
        }

        private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CardInstance ci in e.NewItems)
                {
                    ci.PropertyChanged += ((Pack)sender).RefreshBindings;// CardInstance_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (CardInstance ci in e.OldItems)
                {
                       ci.PropertyChanged -= ((Pack)sender).RefreshBindings;//CardInstance_PropertyChanged;
                }
            }
        }


    }
}
