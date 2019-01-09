using TESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Builders
{
    public class CardInstanceListBuilder
    {
        IEnumerable<CardInstance> cardInstanceList;
        Card card;
        int quantity;

        public CardInstanceListBuilder()
        {
            cardInstanceList = new List<CardInstance>();
        }

        public CardInstanceListBuilder UsingCard(Card card)
        {
            this.card = card;
            return this;
        }

        public CardInstanceListBuilder UsingQuantity(int quantity)
        {
            this.quantity = quantity;
            return this;
        }

        public CardInstanceListBuilder WithCardAndQty(Card card, int quantity, bool isPremium = false)
        {
            var newCardInstances = Enumerable.Range(0, quantity)
               .Select(x => new CardInstanceBuilder()
                           .WithCard(card)
                           .WithQuantity(1)
                           .WithIsPremium(isPremium)
                           .Build()
                       );
            cardInstanceList = cardInstanceList.Union(newCardInstances).ToList();
            return this;
        }

        public IEnumerable<CardInstance> Build()
        {
            return cardInstanceList;
        }
    }
}
