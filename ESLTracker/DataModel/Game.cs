using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.DataModel
{
    public class Game
    {
        [XmlIgnore]
        public Deck Deck;

        private Guid deckId = Guid.Empty;
        public Guid DeckId {
            get
            {
                return (Deck != null) ? Deck.DeckId : deckId ;
            }
            set
            {
                deckId = value;
            }
        }

        public DateTime Date;
        public GameType Type;
        public bool? BonusRound;
        public OrderOfPlay OrderOfPlay;
        public GameOutcome Outcome;

        public DeckAttributes OpponentAttributes = new DeckAttributes();
        public DeckClass OpponentClass;
        public string OpponentName;

        //ranked info
        public PlayerRank? PlayerRank;
        public int? PlayerLegendRank;

        public PlayerRank? OpponentRank;
        public int? OpponentLegendRank;

        public Game()
        {
            Date = DateTime.Now;
        }

    }
}
