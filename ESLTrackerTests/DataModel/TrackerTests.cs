using Microsoft.VisualStudio.TestTools.UnitTesting;
using TESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel.Enums;
using System.Diagnostics;
using ESLTrackerTests;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.Utils.IOWrappers;
using ESLTracker.Utils;
using ESLTrackerTests.Builders;

namespace ESLTracker.DataModel.Tests
{
    [TestClass()]
    public class TrackerTests : BaseTest
    {
        [TestMethod()]
        [Ignore()]
        public void LargeFilesSerialiseDeserialiseSpeedtest()
        {
            //we have to fix up dec ref for each game after load
            //so to test
            //we create file 1 000 000 games
            //1000 decks
            //each game  for random deck
            //then save
            //and load

            int MAX_DECKS = 100;
            int MAX_GAMES = 100000; //50games per day * 365 - cerca 20 000

            string dataFileName = "./data" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml";

            Tracker tracker = new Tracker();
            for (int i = 0; i < MAX_DECKS; i++)
            {
                tracker.Decks.Add(new Deck()
                {
                    Name = "test" + i.ToString("00000"),
                    Class = DeckClass.Agility,
                    Type = DeckType.Constructed,
                });
            }

            for (int i = 0; i < MAX_GAMES; i++)
            {
                tracker.Games.Add(new GameBuilder()
                    .WithBonusRound(false)
                    .WithDeck(tracker.Decks[new Random().Next(MAX_DECKS)])
                    .WithOpponentClass(DeckClass.Spellsword)
                    .WithOpponentLegendRank(1)
                    .WithOpponentName("Game" + i.ToString("0000000000"))
                    .WithOpponentRank(PlayerRank.TheLady)
                    .WithOrderOfPlay(OrderOfPlay.Second)
                    .WithOutcome(GameOutcome.Draw)
                    .WithPlayerLegendRank(1)
                    .WithPlayerRank(PlayerRank.TheLover)
                    .WithType(GameType.PlayRanked)
                    .Build()
                );
            }

            //data ready, lets save!

            new FileSaver(
                new PathManager(mockSettings.Object),
                new PathWrapper(),
                new DirectoryWrapper(),
                new FileWrapper()
                )
                .SaveDatabase<Tracker>(dataFileName, tracker);

            //and let's load!

            Stopwatch sw = new Stopwatch();

            TestContext.WriteLine("Start deserialise....");
            sw.Start();
            Tracker loadedTracker = SerializationHelper.DeserializeXmlPath<Tracker>(dataFileName);

            Assert.AreEqual(MAX_GAMES, loadedTracker.Games.Count);
            Assert.AreEqual(MAX_DECKS, loadedTracker.Decks.Count);

            //fix up ref to decks
            TestContext.WriteLine("file loaded in " + sw.Elapsed.ToString());
            sw.Restart();
            TestContext.WriteLine("Start fix up games");
            foreach (Game g in loadedTracker.Games)
            {
                g.Deck = loadedTracker.Decks.Where(d => d.DeckId == g.DeckId).FirstOrDefault();
                //TestContext.WriteLine("Fix up "+g.DeckId);
            }
            TestContext.WriteLine("Finished in in " + sw.Elapsed.ToString());
            sw.Stop();
        }
    }
}