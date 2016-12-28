using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
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
                return PacksSince(c => c?.Card?.Rarity == CardRarity.Legendary);
            }
        }

        public int MaxPacksSinceLegendary
        {
            get
            {
                return MaxPacksSince(c => c.Card.Rarity == CardRarity.Legendary);
            }
        }

        public int PacksSinceEpic
        {
            get
            {
                return PacksSince(c => c?.Card?.Rarity == CardRarity.Epic);
            }
        }

        public int MaxPacksSinceEpic
        {
            get
            {
                return MaxPacksSince(c => c.Card.Rarity == CardRarity.Epic);
            }
        }

        public int PacksSinceGolden
        {
            get
            {
                return PacksSince(c => c?.IsGolden == true);
            }
        }

        public int MaxPacksSinceGolden
        {
            get
            {
                return MaxPacksSince(c => c?.IsGolden == true);
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

        private int PacksSince(Func<CardInstance, bool> filter)
        {
            return OrderedPacks
                .Where(p => 
                    p.DateOpened > OrderedPacks
                                    .Where(p2 => p2.Cards.Any(filter))
                                    .DefaultIfEmpty(new Pack() { DateOpened = DateTime.MinValue })
                                    .FirstOrDefault()
                                    .DateOpened
                        ).Count();
        }

        private int MaxPacksSince(Func<CardInstance, bool> filter)
        {
            return OrderedPacks
                .Select(p => new
                    {
                        SL = OrderedPacks.Where(p2 => p.DateOpened > p2.DateOpened
                                && p2.DateOpened > (OrderedPacks.Where(
                                p3 => p3.DateOpened < p.DateOpened &&  //get last legendary pack
                                p3.Cards.Any(filter)).DefaultIfEmpty(new Pack() { DateOpened = DateTime.MinValue }).FirstOrDefault().DateOpened)
                            ).Count(),
                    })
                .Max(r => r.SL);
        }
    }
}
