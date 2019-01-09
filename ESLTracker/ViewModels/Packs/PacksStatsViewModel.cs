﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Cards;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Packs
{
    public class PacksStatsViewModel : ViewModelBase
    {
        private IEnumerable<Pack> orderedPacks;

        public IEnumerable<Pack> OrderedPacks
        {
            get
            {
                if (orderedPacks == null)
                {
                    orderedPacks = tracker.Packs.OrderByDescending(p => p.DateOpened);
                }
                return orderedPacks;
            }
            set {
                if (value != null)
                {
                    orderedPacks = value;
                }
                else
                {
                    orderedPacks = tracker.Packs.OrderByDescending(p => p.DateOpened);
                }
                RaisePropertyChangedEvent(String.Empty);
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

        public int PacksSincePremium
        {
            get
            {
                return PacksSince(c => c?.IsPremium == true);
            }
        }

        public int MaxPacksSincePremium
        {
            get
            {
                return MaxPacksSince(c => c?.IsPremium == true);
            }
        }

        public double AveragePackValue
        {
            get
            {
                if (OrderedPacks.Count() > 0)
                {
                    return Math.Round(OrderedPacks.Average(p => soulGemCalculator.CalculateCardsSellValue(p.Cards)), 0);
                }
                else
                {
                    return 0;
                }
            }
        }

        public int NumberOfPacks
        {
            get
            {
                return OrderedPacks.Count();
            }
        }

        public double SoulgemValueTotal
        {
            get
            {
                return OrderedPacks.Sum(p => soulGemCalculator.CalculateCardsSellValue(p.Cards));
            }
        }

        public double? SoulgemValueAvg
        {
            get
            {
                var count = OrderedPacks.Count();
                return count > 0 ? (double?)Math.Round((SoulgemValueTotal / count), 0) : null;
            }
        }

        public int? SoulgemValueMax
        {
            get
            {
                var count = OrderedPacks.Count();
                return count > 0 ? (int?)OrderedPacks.Max(p => soulGemCalculator.CalculateCardsSellValue(p.Cards)) : null;
            }
        }

        public int NumberOfPremiums
        {
            get
            {
                return OrderedPacks.SelectMany(p => p.Cards).Where(ci => ci.IsPremium).Count();
            }
        }
        public double? NumberOfPremiumsPercentage
        {
            get
            {
                var totalCount = OrderedPacks.SelectMany(p => p.Cards).Count();
                var premiumCount = OrderedPacks.SelectMany(p => p.Cards).Where(ci => ci.IsPremium).Count();
                return totalCount > 0 ? (double?)Math.Round( (double)premiumCount/totalCount, 2) : null;
            }
        }

        private ITracker tracker;
        private SoulGemCalculator soulGemCalculator;

        public PacksStatsViewModel(ITracker tracker, SoulGemCalculator soulGemCalculator)
        {
            this.tracker = tracker;
            this.tracker.Packs.CollectionChanged += Packs_CollectionChanged;
            this.soulGemCalculator = soulGemCalculator;
        }

        private void Packs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChangedEvent(String.Empty);
        }

        private int PacksSince(Func<CardInstance, bool> filter)
        {
            if (OrderedPacks.Count() == 0)
            {
                return 0;
            }
            return OrderedPacks
                .Where(p =>
                    p.DateOpened > OrderedPacks
                                    .Where(p2 => p2.Cards.Any(filter))
                                    .Select(p2 => p2.DateOpened)
                                    .DefaultIfEmpty(DateTime.MinValue)
                                    .FirstOrDefault()
                        ).Count();
        }

        private int MaxPacksSince(Func<CardInstance, bool> filter)
        {
            if (OrderedPacks.Count() == 0)
            {
                return 0;
            }

            //indexes of searched packs
            var packIndexes = OrderedPacks.Select((p, index) => new { p, index })
                .Where(col => col.p.Cards.Any(filter))
                .Select(col => col.index);
            if (packIndexes.Count() == 0)
            {
                //nothing fulfills filter - return all!
                return OrderedPacks.Count();
            }
            else if (packIndexes.Count() == 1)
            {
                return packIndexes.First();
            }
            else
            {
                return Math.Max(packIndexes.First(),
                    packIndexes.Zip(packIndexes.Skip(1), (x, y) => y - x).Max()
                    );
            }
        }
    }
}
