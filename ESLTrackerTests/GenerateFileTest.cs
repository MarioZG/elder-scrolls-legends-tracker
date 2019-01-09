using System;
using System.Linq;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.IOWrappers;
using ESLTrackerTests.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESLTrackerTests
{
    [TestClass]
    public class GenerateFileTest : BaseTest
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

            string[] deckTags = {
                "Taahra             ",
                "Bhiila             ",
                "Atrassi            ",
                "Nairanirr          ",
                "Ahjiarra           ",
                "Nisaraji Hamnurabi ",
                "Addhjadhi Zahjspoor",
                "Adannabi Rahknaihn",
                "Faaji Baramanni   ",
                "Abaneena Khaohin  "
            };

            CardInstanceFactory cardFactory = new CardInstanceFactory();
            //DeckVersionFactory deckVersionFactory = new DeckVersionFactory(mockGuidProvider.Object);
            DeckService deckService = new DeckService(null, mockSettings.Object, mockDatetimeProvider.Object, mockGuidProvider.Object);

            foreach (string name in names)
            {
                Deck d = deckService.CreateNewDeck(name);
                d.Type = (DeckType)rand.Next(2);
                d.Class = (DeckClass)rand.Next(16);
                d.CreatedDate = DateTime.Now.AddHours(-1 * rand.Next(5000));
                d.IsHidden = rand.Next(4) < 1 ; //1/4 that is hidden
                d.DeckTag = deckTags[rand.Next(deckTags.Length)];

                int versions = rand.Next(10);
                for (int i = 0; i < versions; i++)
                {
                    DeckVersion version = deckService.CreateDeckVersion(d, 1, i + 1, d.CreatedDate.AddDays(i));
                    int cardCount = 30 + rand.Next(40);
                    for (; cardCount > 0;)
                    {
                        Card c = CardsDatabase.Cards.ElementAt(rand.Next(CardsDatabase.Cards.Count()));
                        int qty = rand.Next(1, 3);
                        version.Cards.Add(cardFactory.CreateFromCard(c, qty));
                        cardCount -= qty;
                    }

                    int gamesCount = rand.Next(300);
                    for (; gamesCount > 0; gamesCount--)
                    {
                        Game g = new GameBuilder()
                            .WithBonusRound(rand.Next(2) % 2 == 1)
                            .WithDate(d.CreatedDate.AddMinutes(rand.Next(10000)))
                            .WithDeck(d)
                            .WithOpponentClass((DeckClass)rand.Next(16))
                            .WithOpponentName("opp1")
                            .WithOrderOfPlay((OrderOfPlay)rand.Next(2))
                            .WithOutcome((GameOutcome)rand.Next(3))
                            .Build();
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
                        g.OpponentDeckTag = deckTags[rand.Next(deckTags.Length)];
                        g.DeckVersionId = version.VersionId;
                        tracker.Games.Add(g);
                    }
                }
                tracker.Decks.Add(d);
            }

            int packsCount = rand.Next(100, 300);
            var sets = CardsDatabase.CardSets.Where(cs => cs.HasPacks);
            for (int i = 0; i < packsCount; i++)
            {
                Pack p = new PackBuilder()
                    .WithDateOpened(DateTime.Now.AddDays(-1 * rand.Next(365)))
                    .WithCardSet(sets.ElementAt(rand.Next(sets.Count())))
                    .Build();

                var cards = CardsDatabase.Cards.Where(c => c.Set == p.CardSet.Name);

                

                p.Cards.Add(cardFactory.CreateFromCard(cards.ElementAt(rand.Next(cards.Count()))));
                p.Cards.Add(cardFactory.CreateFromCard(cards.ElementAt(rand.Next(cards.Count()))));
                p.Cards.Add(cardFactory.CreateFromCard(cards.ElementAt(rand.Next(cards.Count()))));
                p.Cards.Add(cardFactory.CreateFromCard(cards.ElementAt(rand.Next(cards.Count()))));
                p.Cards.Add(cardFactory.CreateFromCard(cards.ElementAt(rand.Next(cards.Count()))));
                p.Cards.Add(cardFactory.CreateFromCard(cards.ElementAt(rand.Next(cards.Count()))));

                tracker.Packs.Add(p);
            }

            tracker.Version = Tracker.CurrentFileVersion;
            new FileSaver(
                new PathManager(mockSettings.Object),
                new PathWrapper(),
                new DirectoryWrapper(),
                new FileWrapper())
                .SaveDatabase(@"c:\dev\aa.xml", tracker);
        }
    }
}
