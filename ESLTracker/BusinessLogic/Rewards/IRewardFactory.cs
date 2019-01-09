using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;

namespace ESLTracker.BusinessLogic.Rewards
{
    public interface IRewardFactory
    {
        Reward CreateReward();
        Reward CreateReward(RewardType rewardType, RewardReason rewardReason);
    }
}