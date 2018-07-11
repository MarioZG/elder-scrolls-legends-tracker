using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using System.Diagnostics;
using ESLTrackerTests;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.Utils.IOWrappers;
using ESLTracker.Utils;

namespace ESLTracker.DataModel.Tests
{
    [TestClass()]
    public class TrackerTests : BaseTest
    {


        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
                    Class = Enums.DeckClass.Agility,
                    Type = Enums.DeckType.Constructed,
                });
            }

            for (int i = 0; i < MAX_GAMES; i++)
            {
                tracker.Games.Add(new Game()
                {
                    BonusRound = false,
                    Deck = tracker.Decks[new Random().Next(MAX_DECKS)],
                    OpponentClass = DeckClass.Spellsword,
                    OpponentLegendRank = 1,
                    OpponentName = "Game" + i.ToString("0000000000"),
                    OpponentRank = PlayerRank.TheLady,
                    OrderOfPlay = OrderOfPlay.Second,
                    Outcome = GameOutcome.Draw,
                    PlayerLegendRank = 1,
                    PlayerRank = PlayerRank.TheLover,
                    Type = GameType.PlayRanked
                });
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