using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Rewards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

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

        [TestMethod()]
        public void SetActiveControlTest001_IsSelectionForGuildVisibleOnlyForGoldAndQuest()
        {

            //if not present in expected array, assume false. Add only true conditions!
            Dictionary<RewardReason, Dictionary<RewardType, bool>> expectedVisibilty = new Dictionary<RewardReason, Dictionary<RewardType, bool>>()
            {
                {RewardReason.Quest, new Dictionary<RewardType, bool>()
                                            {
                                                { RewardType.Card, false },
                                                 { RewardType.Gold, true },
                                                 { RewardType.Pack, false },
                                                 { RewardType.SoulGem, false }
                                            }
                }

            };

            foreach (RewardReason reason in Enum.GetValues(typeof(RewardReason)))
            {
                foreach (RewardType type in Enum.GetValues(typeof(RewardType)))
                {
                    //if not present in expected array, assume false
                    bool expected;
                    if (expectedVisibilty.Keys.Contains(reason)
                        && expectedVisibilty[reason].Keys.Contains(type))
                    {
                        expected = expectedVisibilty[reason][type]; 
                    }
                    else
                    {
                        expected = false;
                    }

                    RewardSetViewModel model = new RewardSetViewModel();
                    model.RewardReason = reason;
                    AddSingleRewardViewModel singleRewardModel = new AddSingleRewardViewModel();
                    singleRewardModel.Reward.Type = type;

                    model.SetActiveControl(singleRewardModel);

                    Assert.AreEqual(expected, singleRewardModel.GuildSelectionVisible, "RewardReason={0}; RewardType={1}", model.RewardReason, singleRewardModel.Reward.Type);

                }
            }

          
        }       
    }
}