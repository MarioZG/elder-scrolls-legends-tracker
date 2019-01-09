using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Rewards
{
    public class RewardFactory : IRewardFactory
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public RewardFactory(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public Reward CreateReward()
        {
            return new Reward()
            {
                Date = dateTimeProvider.DateTimeNow
            };
        }


        public Reward CreateReward(RewardType rewardType, RewardReason rewardReason)
        {
            var reward = CreateReward();
            reward.Type = rewardType;
            reward.Reason = rewardReason;
            return reward;
        }
    }
}
