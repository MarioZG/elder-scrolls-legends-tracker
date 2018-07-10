using ESLTracker.BusinessLogic.Rewards;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Rewards;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.ViewModels.Rewards
{
    [TestClass]
    public class RewardSetViewModelTests : BaseTest
    {

        IAddSingleRewardViewModelFactory mockAddSingleRewardViewModelFactory = new AddSingleRewardViewModelFactory();
        Mock<ITracker> mockTracker = new Mock<ITracker>();
        Mock<IFileManager> mockFileManager = new Mock<IFileManager>();
        IRewardFactory mockRewardFactory;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRewardFactory = new RewardFactory(mockDatetimeProvider.Object);
            new MasserContainer();

        }

        [TestMethod]
        public void RewardSetViewModel001_Init()
        {
            RewardSetViewModel model = CreateRewardSetVM();

            Assert.AreEqual(0, model.RewardsAdded.Count);
            Assert.AreEqual(0, model.RewardsEditor.Count);
            Assert.IsNull(model.RewardReason);
        }


        [TestMethod]
        public void RewardSetViewModel002_ReasonSelected()
        {


            RewardSetViewModel model = CreateRewardSetVM();

            RewardReason reason = RewardReason.LevelUp;

            model.RewardReason = reason;

            Assert.AreEqual(0, model.RewardsAdded.Count);
            Assert.AreEqual(4, model.RewardsEditor.Count);
            Assert.AreEqual(reason, model.RewardReason);
        }

        [TestMethod]
        public void RewardSetViewModel003_ReasonSelectedAndRewardModified()
        {
            RewardSetViewModel model = CreateRewardSetVM();

            ESLTracker.DataModel.Enums.RewardReason reason = ESLTracker.DataModel.Enums.RewardReason.LevelUp;

            model.RewardReason = reason;

            model.RewardsEditor[1].Reward.Quantity = 1;

            Assert.AreEqual(1, model.RewardsAdded.Count);
            Assert.AreEqual(4, model.RewardsEditor.Count);
            Assert.AreEqual(reason, model.RewardReason);
        }

        [TestMethod]
        public void RewardSetViewModel004_ReasonChanged()
        {
            RewardSetViewModel model = CreateRewardSetVM();

            ESLTracker.DataModel.Enums.RewardReason reason = ESLTracker.DataModel.Enums.RewardReason.LevelUp;
            ESLTracker.DataModel.Enums.RewardReason reason2 = ESLTracker.DataModel.Enums.RewardReason.Quest;

            model.RewardReason = reason;

            model.RewardsEditor[1].Reward.Quantity = 1;

            model.RewardReason = reason2;

            model.RewardsEditor[1].Reward.Quantity = 2;

            Assert.AreEqual(2, model.RewardsAdded.Count);
            Assert.AreEqual(4, model.RewardsEditor.Count);
            Assert.AreEqual(1, model.RewardsAdded.Where(r => r.Quantity == 1).Count());
            Assert.AreEqual(1, model.RewardsAdded.Where(r => r.Quantity == 2).Count());
        }

        [TestMethod]
        public void RewardSetViewModel005_ReasonChangedAndChangedBack()
        {
            RewardSetViewModel model = CreateRewardSetVM();

            ESLTracker.DataModel.Enums.RewardReason reason = ESLTracker.DataModel.Enums.RewardReason.LevelUp;
            ESLTracker.DataModel.Enums.RewardReason reason2 = ESLTracker.DataModel.Enums.RewardReason.Quest;

            model.RewardReason = reason;

            model.RewardsEditor[1].Reward.Quantity = 1;

            model.RewardReason = reason2;

            model.RewardsEditor[1].Reward.Quantity = 2;

            model.RewardReason = reason;

            Assert.AreEqual(2, model.RewardsAdded.Count);
            Assert.AreEqual(4, model.RewardsEditor.Count);
            Assert.AreEqual(1, model.RewardsAdded.Where(r => r.Quantity == 1).Count());
            Assert.AreEqual(1, model.RewardsAdded.Where(r => r.Quantity == 2).Count());
        }

        [TestMethod]
        public void RewardSetViewModel006_AddedNewLine()
        {
            RewardSetViewModel model = CreateRewardSetVM();

            RewardReason reason = RewardReason.LevelUp;

            model.RewardReason = reason;

            model.RewardsEditor.Where( r=> r.Reward.Type == RewardType.Gold).Single().Reward.Quantity = 1;

            var reward = model.AddNewReward(RewardType.Gold);
            reward.Quantity = 12;

            Assert.AreEqual(2, model.RewardsAdded.Count);
            Assert.AreEqual(5, model.RewardsEditor.Count);
            Assert.AreEqual(1, model.RewardsAdded.Where(r => r.Quantity == 1).Count());
            Assert.AreEqual(1, model.RewardsAdded.Where(r => r.Quantity == 12).Count());
        }

        [TestMethod]
        public void RewardSetViewModel007_AddedTwoNewLines()
        {
            RewardSetViewModel model = CreateRewardSetVM();

            RewardReason reason = RewardReason.LevelUp;

            model.RewardReason = reason;

            model.RewardsEditor.Where(r => r.Reward.Type == RewardType.Pack).Single().Reward.Quantity = 1;

            var reward = model.AddNewReward(RewardType.Pack);
            reward.Quantity = 2;

            reward = model.AddNewReward(RewardType.Pack);

            var expected = new RewardType[] { RewardType.Gold, RewardType.SoulGem, RewardType.Pack, RewardType.Pack, RewardType.Pack, RewardType.Card };

            Assert.AreEqual(2, model.RewardsAdded.Count);
            Assert.AreEqual(6, model.RewardsEditor.Count);

            CollectionAssert.AreEqual(expected, model.RewardsEditor.Select(r => r.Reward.Type).ToList());

        }

        private RewardSetViewModel CreateRewardSetVM()
        {
            return new RewardSetViewModel(
                mockAddSingleRewardViewModelFactory, 
                mockTracker.Object,
                mockDatetimeProvider.Object, 
                mockFileManager.Object, 
                mockRewardFactory);
        }

    }
}
