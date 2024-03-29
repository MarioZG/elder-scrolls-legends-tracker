﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.ViewModels.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using System.ComponentModel;
using ESLTracker.Properties;
using ESLTrackerTests;
using System.Reflection;
using ESLTracker.Utils;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ESLTracker.Utils.Messages;
using ESLTrackerTests.Builders;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Games;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Game.Tests
{
    [TestClass()]
    public class EditGameViewModelTests : BaseTest
    {
        Mock<IMessenger> messanger = new Mock<IMessenger>();
        Mock<ITracker> tracker = new Mock<ITracker>();
        Mock<IWinAPI> winApi = new Mock<IWinAPI>();
        Mock<IFileSaver> fileSaver = new Mock<IFileSaver>();
        IGameFactory gameFactory;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            gameFactory = new GameFactory(mockDatetimeProvider.Object);
        }

        /// <summary>
        /// verify if last rank selected by player is saved in settings
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest001_SaveSettingForCurrentPlayerRank()
        {


            tracker.Setup(t => t.Games).Returns(new ObservableCollection<TESLTracker.DataModel.Game>());
            tracker.Setup(t => t.ActiveDeck).Returns(new DeckBuilder().Build());

            winApi.Setup(w => w.GetEslFileVersionInfo()).Returns<FileVersionInfo>(null);

            EditGameViewModel model = CreateGameVM();

            PlayerRank selectedPlayerRank = PlayerRank.TheLord;
            model.Game.Type = GameType.PlayRanked;
            model.Game.OpponentClass = DeckClass.Crusader;
            model.Game.PlayerRank = selectedPlayerRank;

            object param = "Victory";

            model.CommandButtonCreateExecute(param);

            mockSettings.VerifySet(s => s.PlayerRank = selectedPlayerRank, Times.Once);

        }



        /// <summary>
        /// verify if last rank selected by player is saved in settings
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest002_RankedFieldsNullForNonRankedGame()
        {
            EditGameViewModel model = CreateGameVM();

            string param = "Victory";

            PopulateObject(model.Game, StartProp);

            Assert.IsNotNull(model.Game.PlayerLegendRank);
            Assert.IsNotNull(model.Game.PlayerRank);
            Assert.IsNotNull(model.Game.OpponentLegendRank);
            Assert.IsNotNull(model.Game.OpponentRank);
            Assert.IsNotNull(model.Game.BonusRound);

            //set type to other than ranked value
            model.Game.Type = GameType.SoloArena;

            model.CommandButtonCreateExecute(param);

            Assert.IsNull(model.Game.PlayerLegendRank);
            Assert.IsNull(model.Game.PlayerRank);
            Assert.IsNull(model.Game.OpponentLegendRank);
            Assert.IsNull(model.Game.OpponentRank);
            Assert.IsNull(model.Game.BonusRound);
        }


        /// <summary>
        /// verify if last rank selected by player is saved in settings
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest003_ValidateRequiredFeilds()
        {
            tracker.Setup(t => t.Games).Returns(new ObservableCollection<TESLTracker.DataModel.Game>());
            tracker.Setup(t => t.ActiveDeck).Returns<Deck>(null);

            EditGameViewModel model = CreateGameVM();

            string param = "Victory";

            model.CommandButtonCreateExecute(param);

            Assert.IsNotNull(model.ErrorMessage);

            messanger.Verify(m => m.Send(It.IsAny<object>()), Times.Never);
        }

        /// <summary>
        /// verify if esl version is added when it's running
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest004_CheckIfESLFileVersionAdded()
        {
            tracker.Setup(t => t.Games).Returns(new ObservableCollection<TESLTracker.DataModel.Game>());
            tracker.Setup(t => t.ActiveDeck).Returns(new DeckBuilder().Build());

            FileVersionInfo expected = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(this.GetType()).Location);

            winApi.Setup(w => w.GetEslFileVersionInfo()).Returns(expected);

            EditGameViewModel model = CreateGameVM();

            GameOutcome param = GameOutcome.Victory;

            model.UpdateGameData(param);

            Assert.IsNotNull(model.Game.ESLVersion);
            Assert.AreEqual(expected.ProductVersion, model.Game.ESLVersion.ToString());

        }

        /// <summary>
        /// verify if ESL version is taken from last game
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest005_CheckIfESLFileVersionAddedWhenESLNOtRunning()
        {
            FileVersionInfo expected = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(this.GetType()).Location);

            DateTime date = new DateTime(2018, 8, 10, 12, 12, 12);

            //add two games wit hdiff version - ensure correct is copied
            tracker.Setup(t => t.Games).Returns(new ObservableCollection<TESLTracker.DataModel.Game>() {
                new GameBuilder().WithDate(date).WithESLVersion(new SerializableVersion(new Version(expected.ProductVersion.ToString()))).Build(),
                new GameBuilder().WithDate(date.AddDays(-7)).WithESLVersion(new SerializableVersion(2, 3)).Build(),
                new GameBuilder().WithDate(date).WithESLVersion(null).Build()
            });
            tracker.Setup(t => t.ActiveDeck).Returns(new DeckBuilder().WithDefaultVersion().Build());

            //ensure not running
            winApi.Setup(w => w.GetEslFileVersionInfo()).Returns<FileVersionInfo>(null);


            EditGameViewModel model = CreateGameVM();

            GameOutcome param = GameOutcome.Victory;

            model.UpdateGameData(param);

            Assert.IsNotNull(model.Game.ESLVersion);
            Assert.AreEqual(expected.ProductVersion, model.Game.ESLVersion.ToString());

        }

        /// <summary>
        /// verify if ESL version is taken from last game
        /// </summary>
        [TestMethod()]
        public void CommandButtonCreateExecuteTest006_CheckIfDeckLastUsedUpdated()
        {

            DateTime gameDate = new DateTime(2018, 8, 10, 21, 32, 0);

            Deck deck = new DeckBuilder().WithDefaultVersion().Build();

            mockDatetimeProvider.Setup(dtp => dtp.DateTimeNow).Returns(gameDate);

            tracker.Setup(t => t.Games).Returns(new GameListBuilder().Build());

            tracker.Setup(t => t.ActiveDeck).Returns(deck);

            EditGameViewModel model = CreateGameVM();

            model.Game = new GameBuilder()
                .WithOutcome(GameOutcome.Victory)
                .WithOpponentClass(DeckClass.Archer)
                .Build();

            GameOutcome param = GameOutcome.Victory;

            model.CommandButtonCreateExecute(param.ToString());

            Assert.AreEqual(gameDate, deck.LastUsed);

        }

        [TestMethod()]
        public void CommandButtonSaveChangesExecuteTest001_RankedFieldsNullForNonRankedGame()
        {
            Mock<ISettings> settingsMock = new Mock<ISettings>();

            EditGameViewModel model = CreateGameVM();

            PopulateObject(model.Game, StartProp);

            Assert.IsNotNull(model.Game.PlayerLegendRank);
            Assert.IsNotNull(model.Game.PlayerRank);
            Assert.IsNotNull(model.Game.OpponentLegendRank);
            Assert.IsNotNull(model.Game.OpponentRank);
            Assert.IsNotNull(model.Game.BonusRound);

            //set type to other than ranked value
            model.Game.Type = GameType.SoloArena;

            model.CommandButtonSaveChangesExecute(null);

            Assert.IsNull(model.Game.PlayerLegendRank);
            Assert.IsNull(model.Game.PlayerRank);
            Assert.IsNull(model.Game.OpponentLegendRank);
            Assert.IsNull(model.Game.OpponentRank);
            Assert.IsNull(model.Game.BonusRound);
        }


        [TestMethod()]
        public void SummaryText001_ChangeEventRaisedWhenNameChanges()
        {
            EditGameViewModel model = CreateGameVM();
            bool raised = false;
            model.PropertyChanged += delegate { raised = true; };

            model.Game.OpponentName = "some name";

            Assert.AreEqual(true, raised);
        }


        [TestMethod()]
        public void SummaryText002_ChangeEventRaisedWhenOpponentClassChanges()
        {
            EditGameViewModel model = CreateGameVM();
            bool raised = false;
            model.PropertyChanged += delegate { raised = true; };

            model.Game.OpponentClass = DeckClass.Endurance;

            Assert.AreEqual(true, raised);
        }

        [TestMethod]
        public void IEditableObjectImplementation001_CancelEdit()
        {
            Mock<IDeckClassSelectorViewModel> deckClassSelector = new Mock<IDeckClassSelectorViewModel>();

            EditGameViewModel model = CreateGameVM();
            TESLTracker.DataModel.Game game = new GameBuilder().Build();

            model.Game = game;

            model.Game.ClearEventInvocations("PropertyChanged");

            PopulateObject(game, StartProp);

            TestContext.WriteLine("Begin Edit");
            model.BeginEdit();

            PopulateObject(game, EditProp);

            TestContext.WriteLine("Cancel Edit");
            model.CancelEdit();

            foreach (PropertyInfo p in game.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    if (p.Name == "DeckId")
                    {
                        //deck id is handled different way, depends on Deck
                        Deck staringDeck = (Deck)StartProp[typeof(Deck)];
                        Assert.AreEqual(staringDeck.DeckId, p.GetValue(game), "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                    }
                    else
                    {
                        Assert.AreEqual(StartProp[p.PropertyType], p.GetValue(game), "Failed validation of prop {0} of type {1}", p.Name, p.PropertyType);
                    }
                }
            }

        }

        [TestMethod]
        public void UpdateGameData001_DateTimeWhenGameConcluded()
        {

            DateTime timeConcluded = new DateTime(2016, 12, 12, 23, 45, 5);

            tracker.Setup(t => t.Games).Returns(new ObservableCollection<TESLTracker.DataModel.Game>());
            tracker.Setup(t => t.ActiveDeck).Returns(new DeckBuilder().Build());

            winApi.Setup(w => w.GetEslFileVersionInfo()).Returns<FileVersionInfo>(null);

            mockDatetimeProvider.Setup(dtp => dtp.DateTimeNow).Returns(timeConcluded);

            EditGameViewModel model = CreateGameVM();

            PopulateObject(model.Game, StartProp);

            Assert.AreNotEqual(timeConcluded, model.Game.Date);

            model.UpdateGameData(GameOutcome.Victory);

            Assert.AreEqual(timeConcluded, model.Game.Date);
        }

        [TestMethod]
        public void ChangeActiveDeck001_AvailableGameTypesUpdated()
        {
            Deck activeDeck = new DeckBuilder().WithType(DeckType.SoloArena).Build();

            tracker.Setup(t => t.ActiveDeck).Returns(activeDeck);

            EditGameViewModel model = CreateGameVM();
            model.ActiveDeckChanged(new ActiveDeckChanged(activeDeck));

            Assert.AreEqual(1, model.AllowedGameTypes.Count());
            Assert.AreEqual(GameType.SoloArena, model.AllowedGameTypes.First());
            Assert.AreEqual(false, model.IsDirty());
        }

        private EditGameViewModel CreateGameVM()
        {
            return new EditGameViewModel(
                tracker.Object, 
                messanger.Object, 
                mockSettings.Object, 
                winApi.Object, 
                fileSaver.Object, 
                mockDatetimeProvider.Object,
                new BusinessLogic.Decks.DeckCalculations(tracker.Object),
                gameFactory);
        }
    }
}