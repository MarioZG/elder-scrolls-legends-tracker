using System;
using System.Collections.Generic;
using System.Linq;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.DataModel
{
    public class Deck
    {
        public Guid DeckId { get; set; }
        public DeckType Type { get; set; }
        public string Name { get; set; }
        public DeckAttributes Attributes { get; set; }
        public DeckClass Class { get; set; }
        public string Notes { get; set; }


        public Deck()
        {
            DeckId = Guid.NewGuid(); //if deserialise, will be overriten!, if new generate!
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

        public IEnumerable<Game> GetDeckGames()
        {
            return Tracker.Instance.Games.Where(g => g.Deck.DeckId == this.DeckId);
        }

        
    }
}