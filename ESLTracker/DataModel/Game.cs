using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.DataModel
{
    public class Game : ViewModels.ViewModelBase
    {
        [XmlIgnore]
        public Deck Deck { get; set; }

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

        public DeckAttributes OpponentAttributes { get; set; } = new DeckAttributes();

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
        public PlayerRank? PlayerRank { get; set; }
        public int? PlayerLegendRank { get; set; }

        public PlayerRank? OpponentRank { get; set; }
        public int? OpponentLegendRank { get; set; }

        public string Notes { get; set; }

        public Game()
        {
            Date = DateTime.Now;
        }

    }
}
