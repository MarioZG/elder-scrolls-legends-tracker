using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Rewards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.ViewModels.Rewards.Tests
{
    [TestClass()]
    public class RewardSetViewModelTests
    {
        [TestMethod()]
        public void DoneClickedTest001_CheckIfAllRewardsHaveSameDate()
        {
            RewardSetViewModel model = new RewardSetViewModel();
            DateTime date = DateTime.Now;

            //set up few with differnt dates
            model.Rewards.Add(new Reward()
            {
                Comment = "test",
                Date = date.AddMinutes(-1)
            });

            model.Rewards.Add(new Reward()
            {
                Comment = "test",
                Date = date.AddDays(-10)
            });

            model.Rewards.Add(new Reward()
            {
                Comment = "test"
            });

            model.DoneClicked(null);

            Assert.AreEqual(Tracker.Instance.Rewards[0].Date, Tracker.Instance.Rewards[1].Date);
            Assert.AreEqual(Tracker.Instance.Rewards[0].Date, Tracker.Instance.Rewards[2].Date);
        }
    }
}