using TESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Windows
{
    public class RewardsSummaryViewModel : ViewModelBase
    {
        public IEnumerable<Reward> Rewards
        {
            get
            {
                return tracker.Rewards;
            }
        }

        ITracker tracker;

        public RewardsSummaryViewModel(ITracker tracker)
        {
            this.tracker = tracker;
        }
    }
}
