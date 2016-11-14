using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Controls.Rewards
{
    public class NewRewardEventArgs : EventArgs
    {
        public DataModel.Reward Reward { get; set; }

        public NewRewardEventArgs(DataModel.Reward reward)
        {
            Reward = reward;
        }
    }
}
