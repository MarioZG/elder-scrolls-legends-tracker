using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Builders
{
    public class PackBuilder
    {
        Pack pack;

        public PackBuilder()
        {
            #pragma warning disable CS0618 // builder class
            pack = new Pack();
            pack.Cards = new ObservableCollection<CardInstance>();
            #pragma warning restore CS0618 // builder class
        }

        public PackBuilder WithCard(CardInstance cardInstance)
        {
            pack.Cards.Add(cardInstance);
            return this;
        }

        public PackBuilder WithDateOpened(DateTime date)
        {
            pack.DateOpened = date;
            return this;
        }

        public PackBuilder WithCardSet(CardSet cardSet)
        {
            pack.CardSet = cardSet;
            return this;
        }

        public Pack Build()
        {
            return pack;
        }
    }
}
