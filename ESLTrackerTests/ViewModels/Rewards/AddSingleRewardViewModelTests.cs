using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using ESLTracker.ViewModels.Rewards;
using ESLTrackerTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ESLTracker.ViewModels.Rewards.Tests
{
    [TestClass()]
    public class AddSingleRewardViewModelTests : BaseTest
    {
        [TestMethod()]
        public void ControlActivatedTest001_FocusOnCorrectElementWhenActivated()
        {
            Dictionary<RewardType, bool> ExpectedQtyFocused = new Dictionary<RewardType, bool>()
            {
                { RewardType.Card, false },
                 { RewardType.Gold, true },
                 { RewardType.Pack, false },
                 { RewardType.SoulGem, true }
            };

            Dictionary<RewardType, bool> ExpectedCommentsFocused = new Dictionary<RewardType, bool>()
            {
                { RewardType.Card, true },
                 { RewardType.Gold, false },
                 { RewardType.Pack, true },
                 { RewardType.SoulGem, false}
            };

            Mock<IRewardSetViewModel> rewardSetModel = new Mock<IRewardSetViewModel>();
            

            foreach (RewardType type in Enum.GetValues(typeof(RewardType)))
            {
                this.TestContext.WriteLine("Starting test for " + type);

                AddSingleRewardViewModel model = new AddSingleRewardViewModel();
                model.ParentDataContext = rewardSetModel.Object;
                model.Type = type;

                model.ControlActivated(null);

                Assert.AreEqual(ExpectedCommentsFocused[type], model.CommentsFocus, "Comments failed for {0}", type);
                Assert.AreEqual(ExpectedQtyFocused[type], model.QtyFocus, "Qty failed for {0}", type);
            }
        }       
    }
}