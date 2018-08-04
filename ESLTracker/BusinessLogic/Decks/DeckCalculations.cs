using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Decks
{
    public class DeckCalculations
    {
        private readonly ITracker tracker;

        public DeckCalculations(ITracker tracker)
        {
            this.tracker = tracker;
        }

        public virtual IEnumerable<Game> GetDeckGames(Deck deck)
        {
            return tracker.Games.Where(g => g.Deck.DeckId == deck?.DeckId);
        }

        public int Victories(Deck deck)
        {
            return Victories(GetDeckGames(deck));
        }
        public int Victories(IEnumerable<Game> games)
        {
            return games.Where(g => g.Outcome == GameOutcome.Victory).Count();
        }

        public int Defeats(Deck deck)
        {
            return Defeats(GetDeckGames(deck));
        }
        public int Defeats(IEnumerable<Game> games)
        {
            return games.Where(g => g.Outcome == GameOutcome.Defeat).Count();
        }

        public int Disconnects(Deck deck)
        {
            return Disconnects(GetDeckGames(deck));
        }
        public int Disconnects(IEnumerable<Game> games)
        {
            return games.Where(g => g.Outcome == GameOutcome.Disconnect).Count();
        }

        public int Draws(Deck deck)
        {
            return Draws(GetDeckGames(deck));
        }
        public int Draws(IEnumerable<Game> games)
        {
            return games.Where(g => g.Outcome == GameOutcome.Draw).Count();
        }

        public string WinRatio(Deck deck)
        {
            var games = GetDeckGames(deck).ToList();
            return WinRatio(games);
        }

        public string WinRatio(IEnumerable<Game> games)
        {
            int gamesTotal = games.Count();
            if (gamesTotal != 0)
            {
                return Math.Round((double)Victories(games) / (double)gamesTotal * 100, 0).ToString();
            }
            else
            {
                return "-";
            }
        }

        public dynamic GetDeckVsClass(Deck deck, DeckClass? opponentClass)
        {
            return this.GetDeckGames(deck)
                      .Where(g => (g.OpponentClass.HasValue)  //filter out all game where calss is not set (if we include in show all, crash below as here is no nul key in classes.attibutes)
                              && ((g.OpponentClass == opponentClass) || (opponentClass == null))) //class = param, or oaram is null - show all"
                      .GroupBy(d => d.OpponentClass.Value)
                      .Select(d => new
                      {
                          Class = d.Key,
                          Attributes = ClassAttributesHelper.Classes[d.Key],
                          Total = d.Count(),
                          Victory = d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count(),
                          Defeat = d.Where(d2 => d2.Outcome == GameOutcome.Defeat).Count(),
                          WinPercent = d.Count() > 0 ? Math.Round((decimal)d.Where(d2 => d2.Outcome == GameOutcome.Victory).Count() / (decimal)d.Count() * 100, 0).ToString() : "-"
                      });
        }

        public bool IsArenaRunFinished(Deck deck)
        {
            switch (deck.Type)
            {
                case DeckType.Constructed:
                    return false;
                case DeckType.VersusArena:
                    return
                        Victories(deck) == 7
                        || Defeats(deck) + Disconnects(deck) == 3;
                case DeckType.SoloArena:
                    return
                        Victories(deck) == 9
                        || Defeats(deck) + Disconnects(deck) == 3;
                default:
                    throw new NotImplementedException("Is arena run finished not dfined for type {" + deck.Type + "}");
            }
        }

        public IEnumerable<bool> GetLastGames(Deck deck, int gamesCount)
        {
            IEnumerable<bool> wins = GetDeckGames(deck)
                .OrderByDescending(g => g.Date)
                .Take(gamesCount)
                .Reverse()
                .Select(g => g.Outcome == GameOutcome.Victory)
                .ToList();

            return wins;
        }

        public IEnumerable<Reward> GetArenaRewards(Guid deckId)
        {
            return tracker.Rewards.Where(r => r.ArenaDeckId == deckId);
        }

    }
}
