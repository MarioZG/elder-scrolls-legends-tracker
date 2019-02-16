using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Cards;

namespace ESLTracker.BusinessLogic.Decks
{
    public class CardBreakdown
    {
        private readonly DoubleCardsCalculator doubleCardsCalculator;

        public CardBreakdown(DoubleCardsCalculator doubleCardsCalculator)
        {
            this.doubleCardsCalculator = doubleCardsCalculator;
        }

        public IEnumerable<KeyValuePair<DeckAttribute, int>> GetCardsColorBreakdown(IEnumerable<CardInstance> cards)
        {
            var attributeBreakDown = cards
                .SelectMany(ci => Enumerable.Repeat(ci.Card.Attributes, GetCardQuantityForAttributeBreakdown(ci)))
                .SelectMany(kl => kl)
                .GroupBy(
                    ck => ck,
                    ck => ck,
                    (key, g) => new KeyValuePair<DeckAttribute, int>(key, (int)g.Count())
                    )
                    ;
            return attributeBreakDown;

        }

        public IEnumerable<KeyValuePair<int, int>> GetManaBreakdown(IEnumerable<CardInstance> cards)
        {
            var manaBreakdown = Enumerable.Range(0, 7).ToDictionary( 
                (i) => i,
                (i) => cards.Where(ci => ci.Card.Cost == i).Sum(ci => ci.Quantity))
                .Union( new[] { new KeyValuePair<int, int>(7, cards.Where(ci => ci.Card.Cost >= 7).Sum(ci => ci.Quantity)) });

            return manaBreakdown;
        }

        public IEnumerable<KeyValuePair<CardType, int>> GetCardTypeBreakdown(IEnumerable<CardInstance> cards)
        {
            var cardTypeBreakdown =
                cards
                .GroupBy(
                    i => i.Card.Type,
                    i => GetCardQuantityForTypeBreakdown(i),
                    (type, count) => new KeyValuePair<CardType, int>(type, (int)count.Sum(c => c)));


            return cardTypeBreakdown;
        }


        public IEnumerable<KeyValuePair<CardKeyword, int>> GetCardKeywordsBreakdown(IEnumerable<CardInstance> cards)
        {
            var breakdown = cards
                        .SelectMany(ci => Enumerable.Repeat(ci.Card.Keywords, ci.Quantity))
                        .SelectMany(keyword => keyword)
                        .GroupBy(
                            keyword => keyword,
                            (keyword, count) => new KeyValuePair<CardKeyword, int>(keyword, count.Count())
                        );

            return breakdown;
        }

        public IEnumerable<KeyValuePair<T, int>> GetCardKeywordsBreakdown<T>(IEnumerable<CardInstance> cards, Func<Card,List<T>> keywordsSelector)
        {
            var breakdown = cards
                        .SelectMany(ci => Enumerable.Repeat(keywordsSelector(ci.Card), ci.Quantity))
                        .SelectMany(keyword => keyword)
                        .GroupBy(
                            keyword => keyword,
                            (keyword, count) => new KeyValuePair<T, int>(keyword, count.Count())
                        );

            return breakdown;
        }

        public int? GetTotalCount(IEnumerable<CardInstance> cards)
        {
            return (int?)cards?.Sum(c => GetCardQuantity(c));
        }

        private decimal GetCardQuantity(CardInstance c)
        {
            return c.Card.DoubleCard != null ? c.Quantity / 2m : c.Quantity;
        }

        private decimal GetCardQuantityForTypeBreakdown(CardInstance c)
        {
            return c.Card.DoubleCard != null ? doubleCardsCalculator.GetTypeCount(c.CardId) : c.Quantity;
        }

        private int GetCardQuantityForAttributeBreakdown(CardInstance c)
        {
            return c.Card.DoubleCard != null ? doubleCardsCalculator.GetAttribute(c.CardId) : c.Quantity;
        }
    }
}
