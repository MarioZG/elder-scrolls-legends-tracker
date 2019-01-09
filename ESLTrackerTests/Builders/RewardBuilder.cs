using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Builders
{
    public class RewardBuilder
    {
        Reward reward;

        public RewardBuilder()
        {
            reward = new Reward();
        }

        public RewardBuilder WithDeck(Deck deck)
        {
            reward.ArenaDeck = deck;
            return this;
        }

        public RewardBuilder WithType(RewardType type)
        {
            reward.Type = type;
            return this;
        }

        public RewardBuilder WithQuantity(int quantity)
        {
            reward.Quantity = quantity;
            return this;
        }

        public RewardBuilder WithCardInstance(CardInstance cardInstance)
        {
            reward.CardInstance = cardInstance;
            return this;
        }

        public Reward Build()
        {
            return reward;
        }
    }
}
