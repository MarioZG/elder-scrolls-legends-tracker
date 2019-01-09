using TESLTracker.DataModel;
using ESLTracker.ViewModels.Rewards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Rewards
{
    public interface IAddSingleRewardViewModelFactory
    {
        AddSingleRewardViewModel Create(Reward reward, RewardSetViewModel parentViewModel);
    }
}
