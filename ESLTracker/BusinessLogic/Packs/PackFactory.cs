using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Packs
{
    public class PackFactory
    {

        public const int PackSize = 6;
        IEnumerable<CardInstance> CardsForEmptyPack => Enumerable.Range(1, PackSize).Select(i => new CardInstance());

        public Pack CreatePack(
            IEnumerable<CardInstance> startringCards, 
            bool raiseChangeEventsOnCards, 
            PropertyChangedEventHandler refreshHandler)
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
                SetUpChangeEvents(pack, refreshHandler);
            }

            return pack;
        }

        public Pack CreateEmptyPack(
            bool raiseChangeEventsOnCards,
            PropertyChangedEventHandler refreshHandler)
        {
            return CreatePack(
                CardsForEmptyPack,
                raiseChangeEventsOnCards,
                refreshHandler);
        }

        public void ClearPack(Pack pack, PropertyChangedEventHandler refreshHandler)
        {
            pack.Cards = new ObservableCollection<CardInstance>(CardsForEmptyPack);
            SetUpChangeEvents(pack, refreshHandler);
        }

        private void SetUpChangeEvents(Pack pack, PropertyChangedEventHandler refreshHandler)
        {
            pack.Cards.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.NewItems != null)
                {
                    foreach (CardInstance ci in e.NewItems)
                    {
                        ci.PropertyChanged += refreshHandler;
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (CardInstance ci in e.OldItems)
                    {
                        ci.PropertyChanged -= refreshHandler;
                    }
                }
            };

            if (pack.Cards != null)
            {
                pack.Cards.All(c => { c.PropertyChanged += refreshHandler; return true; });
            }
        }
    }
}
