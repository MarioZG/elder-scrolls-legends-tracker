using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Packs
{
    public class PacksStatsViewModel : ViewModelBase
    {
        public IEnumerable<Pack> OrderedPacks
        {
            get
            {
                return trackerFactory.GetTracker().Packs.OrderByDescending(p => p.DateOpened);
            }
        }

        public int PacksSinceLegendary
        {
            get
            {
                return OrderedPacks.Where(p=> p.DateOpened > OrderedPacks.Where( p2=> p2.Cards.Any( c=> c?.Card?.Rarity == DataModel.Enums.CardRarity.Legendary)).DefaultIfEmpty(new Pack() { DateOpened = DateTime.MinValue }).FirstOrDefault().DateOpened).Count();
            }
        }

        public int PacksSinceEpic
        {
            get
            {
                return OrderedPacks.Where(p => p.DateOpened > OrderedPacks.Where(p2 => p2.Cards.Any(c => c?.Card?.Rarity == DataModel.Enums.CardRarity.Epic)).DefaultIfEmpty(new Pack() { DateOpened = DateTime.MinValue }).FirstOrDefault().DateOpened).Count();
            }
        }

        public int PacksSinceGolden
        {
            get
            {
                return OrderedPacks.Where(p => p.DateOpened > OrderedPacks.Where(p2 => p2.Cards.Any(c => c?.IsGolden == true)).DefaultIfEmpty(new Pack() { DateOpened = DateTime.MinValue }).FirstOrDefault().DateOpened).Count();
            }
        }

        public double AveragePackValue
        {
            get
            {
                if (OrderedPacks.Count() > 0)
                {
                    return Math.Round(OrderedPacks.Average(p => p.SoulGemsValue), 0);
                }
                else
                {
                    return 0;
                }
            }
        }

        private TrackerFactory trackerFactory;

        public PacksStatsViewModel() : this(new TrackerFactory())
        {

        }

        public PacksStatsViewModel(TrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            this.trackerFactory.GetTracker().Packs.CollectionChanged += Packs_CollectionChanged;
        }

        private void Packs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChangedEvent("");
        }
    }
}
