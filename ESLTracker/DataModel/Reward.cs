using System;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.DataModel
{
    public class Reward
    {
        public Reward()
        {
            Date = DateTime.Now;
        }

        public RewardReason Reason { get; set; }
        public RewardType Type { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; }

        //quest rewards
        public Guild? RewardQuestGuild {get;set;}
    }
}