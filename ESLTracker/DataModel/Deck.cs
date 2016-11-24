using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.DataModel
{
    public class Deck
    {
        public Guid DeckId { get; set; }
        public DeckType Type { get; set; }
        public string Name { get; set; }
        public DeckAttributes Attributes { get; set; } = new DeckAttributes();
        public DeckClass Class { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }


        public Deck()
        {
            DeckId = Guid.NewGuid(); //if deserialise, will be overriten!, if new generate!
            CreatedDate = DateTime.Now;
        }

        public int Victories {
            get {
                return GetDeckGames().Where(g => g.Outcome == GameOutcome.Victory).Count();
            }
        }

        public int Defeats
        {
            get
            {
                return GetDeckGames().Where(g => g.Outcome == GameOutcome.Defeat).Count();
            }
        }

        public string WinRatio
        {
            get
            {
                int gamesTotal = GetDeckGames().Count();
                if (gamesTotal != 0)
                {
                    return Math.Round((double)Victories / (double)GetDeckGames().Count() * 100, 0).ToString();
                }
                else
                {
                    return "-";
                }
            }
        }

        public IEnumerable<Game> GetDeckGames()
        {
            return Tracker.Instance.Games.Where(g => g.Deck.DeckId == this.DeckId);
        }

        public IEnumerable GetDeckVsClass()
        {
            return GetDeckVsClass(null);
        }

        public IEnumerable GetDeckVsClass(DeckClass? opponentClass)
        {
            return this.GetDeckGames()
                        .Where(g=> (g.OpponentClass.HasValue)  //filter out all game where calss is not set (if we include in show all, crash below as here is no nul key in classes.attibutes)
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

        
    }
}