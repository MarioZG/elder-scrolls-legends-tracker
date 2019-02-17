using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel.Enums;

namespace ESLTracker.BusinessLogic.Cards
{
    public class DoubleCardsCalculator
    {
        private readonly ICardsDatabase cardsDatabase;

        private Dictionary<Guid, decimal> cardsByTypeCount = new Dictionary<Guid, decimal>();
        private Dictionary<Guid, int> cardsByAttribute = new Dictionary<Guid, int>();  //onw ill have 1 other zer whn same colour

        public DoubleCardsCalculator(ICardsDatabaseFactory cardsDatabaseFactory)
        {
            this.cardsDatabase = cardsDatabaseFactory.GetCardsDatabase();
            Init();
        }

        public decimal GetTypeCount(Guid cardId)
        {
            return cardsByTypeCount[cardId];
        }

        public int GetAttribute(Guid cardId)
        {
            return cardsByAttribute[cardId];
        }

        private void Init()
        {
            var doubles = cardsDatabase.Cards.Where(c => c.Type == CardType.Double);
            foreach(var d in doubles)
            {
                var cards = cardsDatabase.Cards.Where(c => d.DoubleCardComponents.Contains(c.Id)).ToList();
                if (cards.Count != 2)
                {
                    throw new DoubleCardException($"Card {d.Name} doesnt have 2 components");
                }
                CountByType(cards);
                CountByAttribute(cards);
            }
        }

        private void CountByType(List<TESLTracker.DataModel.Card> cards)
        {
            var count = cards.First().Type == cards.Last().Type ? 0.5m : 1;
            cardsByTypeCount.Add(cards.First().Id, count);
            cardsByTypeCount.Add(cards.Last().Id, count);
        }

        private void CountByAttribute(List<TESLTracker.DataModel.Card> cards)
        {
            var areSame = cards.First().Attributes.Single() == cards.Last().Attributes.Single();
            cardsByAttribute.Add(cards.First().Id, areSame ? 0 : 1);  //if same first one will have zero
            cardsByAttribute.Add(cards.Last().Id, 1);
        }
    }
}
