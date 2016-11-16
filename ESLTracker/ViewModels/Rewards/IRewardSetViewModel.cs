using ESLTracker.DataModel;

namespace ESLTracker.ViewModels.Rewards
{
    public interface IRewardSetViewModel
    {
        void AddReward(Reward reward);
        void DoneClicked(object param);
        void RegisterControl(AddSingleRewardViewModel c);
        void RewardReasonChanged();
        void SetActiveControl(AddSingleRewardViewModel c);
    }
}