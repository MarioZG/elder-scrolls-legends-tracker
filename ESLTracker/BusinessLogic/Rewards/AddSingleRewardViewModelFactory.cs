using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Rewards;

namespace ESLTracker.BusinessLogic.Rewards
{
    public class AddSingleRewardViewModelFactory : IAddSingleRewardViewModelFactory
    {
        public AddSingleRewardViewModel Create(Reward reward, RewardSetViewModel parentViewModel)
        {
            var singleRewardViewModel =  MasserContainer.Container.GetInstance<AddSingleRewardViewModel>();
            singleRewardViewModel.Reward = reward;
            singleRewardViewModel.ParentRewardViewModel = parentViewModel;

            return singleRewardViewModel;
        }
    }
}
