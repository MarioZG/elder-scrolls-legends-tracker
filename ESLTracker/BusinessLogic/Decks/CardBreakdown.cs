using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Decks
{
    public class CardBreakdown
    {
        public IEnumerable<KeyValuePair<DeckAttribute, int>> GetCardsColorBreakdown(IEnumerable<CardInstance> cards)
        {
            var attributeBreakDown = cards
                .SelectMany(ci => Enumerable.Repeat(ci.Card.Attributes, ci.Quantity))
                .SelectMany(kl => kl)
                .GroupBy(
                    ck => ck,
                    ck => ck,
                    (key, g) => new KeyValuePair<DeckAttribute, int>(key, g.Count())
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
            var manaBreakdown =
                cards
                .GroupBy(
                    i => i.Card.Type,
                    i => i.Quantity,
                    (type, count) => new KeyValuePair<CardType, int>(type, count.Sum(c => c)));


            return manaBreakdown;
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
    }
}
