using System;
using System.Linq;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Services;
using ESLTracker.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESLTrackerTests
{
    [TestClass]
    public class GenerateFileTest
    {

        Random rand = new Random((int)DateTime.Now.Ticks);

        [TestMethod]
        [Ignore] //"Run only on demand!"
        public void GenerateFile()
        {
            Tracker tracker = new Tracker();

            string[] names = new string[]
            {
                "Denita",
                "Kristopher",
                "Danita    ",
                "Marjory   ",
                "Elvera    ",
                "Myrle     ",
                "Sarai     ",
                "Rodney    ",
                "Ian       ",
                "Doreen    ",
                "Fatima    ",
                "Leeanne   ",
                "Shae      ",
                "Marianna  ",
                "Roselee   ",
                "Sid       ",
                "Nicolette ",
                "Brandee   ",
                "Sandee    ",
                "Bradly    "
            };

            foreach (string name in names)
            {
                Deck d = Deck.CreateNewDeck(name);
                d.Type = (DeckType)rand.Next(2);
                d.Class = (DeckClass)rand.Next(16);
                d.CreatedDate = DateTime.Now.AddHours(-1 * rand.Next(5000));
                d.IsHidden = rand.Next(2) % 2 == 1;

                int versions = rand.Next(10);
                for (int i = 0; i < versions; i++)
                {
                    DeckVersion version = d.CreateVersion(1, i + 1, d.CreatedDate.AddDays(i));
                    int cardCount = 30 + rand.Next(40);
                    for (; cardCount > 0; cardCount--)
                    {
                        Card c = CardsDatabase.Default.Cards.ElementAt(rand.Next(CardsDatabase.Default.Cards.Count()));
                        version.Cards.Add(new CardInstance(c));
                    }

                    int gamesCount = rand.Next(100);
                    for (; gamesCount > 0; gamesCount--)
                    {
                        Game g = new Game();
                        g.BonusRound = rand.Next(2) % 2 == 1;
                        g.Date = d.CreatedDate.AddMinutes(rand.Next(10000));
                        g.Deck = d;
                        g.OpponentClass = (DeckClass)rand.Next(16);
                        g.OpponentName = "opp1";
                        g.OrderOfPlay = (OrderOfPlay)rand.Next(2);
                        g.Outcome = (GameOutcome)rand.Next(3);
                        switch (d.Type)
                        {
                            case DeckType.Constructed:
                                g.Type = rand.Next(2) % 2 == 1 ? GameType.PlayCasual : GameType.PlayRanked;
                                break;
                            case DeckType.VersusArena:
                                g.Type = GameType.VersusArena;
                                break;
                            case DeckType.SoloArena:
                                g.Type = GameType.SoloArena;
                                break;
                            default:
                                break;
                        }
                        g.DeckVersionId = version.VersionId;
                        tracker.Games.Add(g);
                    }
                }
                tracker.Decks.Add(d);
            }

            tracker.Version = Tracker.CurrentFileVersion;
            new FileManager().SaveDatabase(@"c:\dev\aa.xml", tracker);
        }
    }
}
