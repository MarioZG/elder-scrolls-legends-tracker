﻿using ESLTracker.DataModel;
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
            return GetDeckGames(deck).Where(g => g.Outcome == GameOutcome.Victory).Count();
        }

        public int Defeats(Deck deck)
        {
            return GetDeckGames(deck).Where(g => g.Outcome == GameOutcome.Defeat).Count();
        }

        public int Disconnects(Deck deck)
        {
            return GetDeckGames(deck).Where(g => g.Outcome == GameOutcome.Disconnect).Count();
        }
        public int Draws(Deck deck)
        {
            return GetDeckGames(deck).Where(g => g.Outcome == GameOutcome.Draw).Count();
        }

        public string WinRatio(Deck deck)
        {
            int gamesTotal = GetDeckGames(deck).Count();
            if (gamesTotal != 0)
            {
                return Math.Round((double)Victories(deck) / (double)gamesTotal * 100, 0).ToString();
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

    }
}