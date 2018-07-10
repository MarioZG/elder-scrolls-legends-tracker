using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Builders
{
    public class CardBuilder
    {
        Card card;

        public CardBuilder()
        {
            card = new Card(); 
        }

        public CardBuilder WithId(Guid cardId)
        {
            card.Id = cardId;
            return this;
        }

        public CardBuilder WithIsUnique(bool isUnique)
        {
            card.IsUnique = isUnique;
            return this;
        }

        public CardBuilder WithName(string name)
        {
            card.Name = name;
            return this;
        }

        public Card Build()
        {
            return card;
        }
    }
}
