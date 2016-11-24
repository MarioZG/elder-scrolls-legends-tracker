using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.ComponentModel;
using ESLTracker.Properties;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.ViewModels.Game.Tests
{
    [TestClass()]
    public class EditGameViewModelTests
    {
        /// <summary>
        /// verify if last rank selected by player is saved in settings
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest001_SaveSettingForCurrentPlayerRank()
        {
            Mock<ISettings> settingsMock = new Mock<ISettings>();

            EditGameViewModel model = new EditGameViewModel();

            PlayerRank selectedPlayerRank = PlayerRank.TheLord;
            model.Game.Type = GameType.PlayRanked;

            object[] args =
            {
                "Victory",
                model,
                new DeckClassSelectorViewModel() { SelectedClass = DeckClass.Crusader},
                new Controls.PlayerRank(),
                new Controls.PlayerRank() {SelectedItem =  selectedPlayerRank}
            };

            model.CommandButtonCreateExecute(args, settingsMock.Object);

            settingsMock.VerifySet(s => s.PlayerRank = selectedPlayerRank, Times.Once);

        }

        [TestMethod()]
        public void SummaryText001_ChangeEventRaisedWhenNameChanges()
        {
            EditGameViewModel model = new EditGameViewModel();
            bool raised = false;
            model.PropertyChanged += delegate { raised = true; }; 

            model.Game.OpponentName = "some name";

            Assert.AreEqual(true, raised);
        }


        [TestMethod()]
        public void SummaryText002_ChangeEventRaisedWhenOpponentClassChanges()
        {
            EditGameViewModel model = new EditGameViewModel();
            bool raised = false;
            model.PropertyChanged += delegate { raised = true; };

            model.Game.OpponentClass = DeckClass.Endurance;

            Assert.AreEqual(true, raised);
        }


    }
}