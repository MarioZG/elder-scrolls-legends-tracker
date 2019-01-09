using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;

namespace ESLTracker.Controls.Rewards
{
    public class NewRewardEventArgs : EventArgs
    {
        public Reward Reward { get; set; }

        public NewRewardEventArgs(Reward reward)
        {
            Reward = reward;
        }
    }
}
