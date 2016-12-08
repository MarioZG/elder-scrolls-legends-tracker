using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.DataModel
{
    public class Game : ViewModels.ViewModelBase, ICloneable
    {
        private Deck deck;

        [XmlIgnore]
        public Deck Deck
        {
            get { return deck; }
            set { deck = value; RaisePropertyChangedEvent("Deck"); }
        }


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

        public DateTime Date { get; set; }

        GameType? type;
        public GameType? Type {
            get { return type; }
            set
            {
                type = value;
                RaisePropertyChangedEvent("Type");
            }
        }

        bool? bonusRound;
        public bool? BonusRound
        {
            get { return bonusRound; }
            set
            {
                bonusRound = value;
                RaisePropertyChangedEvent("BonusRound");
            }
        }

        public OrderOfPlay? OrderOfPlay { get; set; }
        public GameOutcome Outcome { get; set; }

        DeckClass? opponentClass;
        public DeckClass? OpponentClass
        {
            get { return opponentClass; }
            set { opponentClass = value; RaisePropertyChangedEvent("OpponentClass"); }
        }

        string opponentName;
        public string OpponentName
        {
            get { return opponentName; }
            set { opponentName = value; RaisePropertyChangedEvent("OpponentName"); }
        }

        //ranked info
        private PlayerRank? playerRank;
        public PlayerRank? PlayerRank
        {
            get { return playerRank; }
            set { playerRank = value; RaisePropertyChangedEvent("PlayerRank"); }
        }

        public int? PlayerLegendRank { get; set; }

        public PlayerRank? OpponentRank { get; set; }
        public int? OpponentLegendRank { get; set; }

        public string Notes { get; set; }

        public Game() : this(new TrackerFactory())
        {
        }

        public Game(ITrackerFactory factory)
        {
            Date = factory.GetDateTimeNow();
        }

        public void UpdateAllBindings()
        {
            RaisePropertyChangedEvent("");
        }

        public object Clone()
        {
            return this.MemberwiseClone() as Game;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Game g = obj as Game;
            if ((System.Object)g == null)
            {
                return false;
            }

            return (this.BonusRound == g.BonusRound
                 && this.Date == g.Date
                 && this.Deck == g.Deck
                 && this.DeckId == g.DeckId
                 && this.Notes == g.Notes
                // && this.OpponentAttributes == g.OpponentAttributes
                 && this.OpponentClass == g.OpponentClass
                 && this.OpponentLegendRank == g.OpponentLegendRank
                 && this.OpponentName == g.OpponentName
                 && this.OpponentRank == g.OpponentRank
                 && this.OrderOfPlay == g.OrderOfPlay
                 && this.Outcome == g.Outcome
                 && this.PlayerLegendRank == g.PlayerLegendRank
                 && this.PlayerRank == g.PlayerRank
                 && this.Type == g.Type);
        }
    }
}
