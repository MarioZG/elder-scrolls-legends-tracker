using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Packs
{
    public class CardSetsListProvider
    {

        public static CardSet AllSets { get; } = new CardSet
        {
            Id = Guid.Empty,
            Name = "All",
        };

        private readonly ICardsDatabaseFactory cardsDatabaseFactory;

        public CardSetsListProvider(ICardsDatabaseFactory cardsDatabaseFactory)
        {
            this.cardsDatabaseFactory = cardsDatabaseFactory;
        }

        public IEnumerable<CardSet> GetCardSetList(bool addAllOptionAsFisrt)
        {
            List<CardSet> packs = cardsDatabaseFactory.GetCardsDatabase()
                            .CardSets
                            .Where(cs => cs.HasPacks)
                            .ToList();
            if (addAllOptionAsFisrt)
            {
                packs.Insert(0, CardSetsListProvider.AllSets);
            }
            return packs;

        }
    }
}
