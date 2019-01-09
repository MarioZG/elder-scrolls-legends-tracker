using ESLTracker.BusinessLogic.Cards;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Packs
{
    public class PacksChartsDataCalculator
    {
        public struct AttriuteQtyData
        {
            public DeckAttribute Attribute;
            public decimal Qty;
        };

        private readonly IDataToSeriesTranslator dataToSeriesTranslator;
        private readonly ICardInstanceFactory cardInstanceFactory;
        private readonly ICardImage cardImageService;

        public PacksChartsDataCalculator(ICardImage cardImageService, 
            ICardInstanceFactory cardInstanceFactory,
            IDataToSeriesTranslator dataToSeriesTranslator)
        {
            this.cardImageService = cardImageService;
            this.dataToSeriesTranslator = dataToSeriesTranslator;
            this.cardInstanceFactory = cardInstanceFactory;
        }

        public object GetPieChartByClassData(IEnumerable<CardInstance> cardsData)
        {
            var rawData = cardsData
                  .SelectMany(c => c.Card.Attributes, (ci, a) => new AttriuteQtyData
                  {
                      Attribute = a,
                      Qty = (decimal)1 / ci.Card.Attributes.Count //adjust for multi-attributes
                  });
            decimal totalCount = rawData.Sum(d => d.Qty);
            var data = rawData
                .GroupBy(c => c.Attribute)
                .OrderBy(c => c.Key)
                .Select(c => GetPieChartByClassSeries(c, totalCount))
                .ToList();

            return dataToSeriesTranslator.CreateSeriesCollection(data);
        }

        private object GetPieChartByClassSeries(IGrouping<DeckAttribute, AttriuteQtyData> group, decimal totalCount)
        {
            DeckAttribute attrib = group.Key;
            var attribSum = group.Sum(d => d.Qty);
            string title = $"{ attrib} { Math.Round(attribSum / totalCount * 100, 2)}% ({attribSum.ToString("0.#")})";

            return dataToSeriesTranslator.CreateSeries(
                title,
                ClassAttributesHelper.DeckAttributeColors[attrib].ToMediaBrush(),
                attribSum);
        }

        public object GetPieChartByRarityData(IEnumerable<CardInstance> cardsData)
        {
            var rawData = cardsData.Select(c => c.Card.Rarity);
            int totalCount = rawData.Count();
            var data = rawData
                .GroupBy(c => c)
                .OrderBy(c => c.Key)
                .Select(c => GetPieChartByRaritySeries(c, totalCount))
                .ToList();

            return dataToSeriesTranslator.CreateSeriesCollection(data);
        }

        private object GetPieChartByRaritySeries(IGrouping<CardRarity, CardRarity> group, decimal totalCount)
        {
            CardRarity rarity = group.Key;
            var rarityCount = group.Count();
            string title = $"{ rarity } { Math.Round((decimal)rarityCount / totalCount * 100, 2)}% ({rarityCount})";
            return dataToSeriesTranslator.CreateSeries(
                title,
                cardImageService.GetRarityBrush(rarity),
                rarityCount);

        }

        public object GetPieChartPremiumByRarityData(IEnumerable<CardInstance> cardsData)
        {
            var rawData = cardsData
                .Where(ci => ci.IsPremium)
                .Select(c => c.Card.Rarity);
            int totalCount = rawData.Count();
            var data = rawData
                .GroupBy(c => c)
                .OrderBy(c => c.Key)
                .Select(c => GetPieChartByRaritySeries(c, totalCount))
                .ToList();

            return dataToSeriesTranslator.CreateSeriesCollection(data);
        }

        public IEnumerable<CardInstance> GetTopXCardsData(IEnumerable<CardInstance> cardsData, int topX)
        {
            return cardsData
                .GroupBy(ci => ci.Card)
                .Select(ci => cardInstanceFactory.CreateFromCard(ci.Key, ci.Count()))
                .OrderByDescending(cis => cis.Quantity)
                .Take(topX)
                .ToList();
        }
    }
}
