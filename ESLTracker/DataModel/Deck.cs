using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.DataModel
{
    public class Deck : ViewModels.ViewModelBase, ICloneable
    {
        public Guid DeckId { get; set; }

        private DeckType type;
        public DeckType Type
        {
            get { return type; }
            set { type = value; RaisePropertyChangedEvent("Type"); }
        }

        public string Name { get; set; }

        [XmlIgnore]
        public DeckAttributes Attributes
        {
            get
            {
                if (Class.HasValue)
                {
                    return new DeckAttributes(ClassAttributesHelper.Classes[Class.Value]);
                }
                else
                {
                    return new DeckAttributes();
                }
            }
        }

        public DeckClass? Class { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public ArenaRank? ArenaRank { get; set; }

        private ITrackerFactory tracker; //cannot be ITracker, as we need to load it first - stack overflow when database is loading

        public Deck() : this(new TrackerFactory())
        {
        }

        internal Deck(ITrackerFactory tracker)
        {
            DeckId = Guid.NewGuid(); //if deserialise, will be overriten!, if new generate!
            CreatedDate = tracker.GetDateTimeNow();
            this.tracker = tracker;
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

        public int Disconnects {
            get
            {
                return GetDeckGames().Where(g => g.Outcome == GameOutcome.Disconnect).Count();
            }
        }
        public int Draws
        {
            get
            {
                return GetDeckGames().Where(g => g.Outcome == GameOutcome.Draw).Count();
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
            return tracker.GetTracker().Games.Where(g => g.Deck.DeckId == this.DeckId);
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

        public void UpdateAllBindings()
        {
            RaisePropertyChangedEvent("");
        }

        public object Clone()
        {
            return this.MemberwiseClone() as Deck;
        }

        public static bool IsArenaDeck(DeckType type)
        {
            return type == DeckType.SoloArena || type == DeckType.VersusArena;
        }

        public bool IsArenaRunFinished()
        {
            switch (this.Type)
            {
                case DeckType.Constructed:
                    return false;
                case DeckType.VersusArena:
                    return
                        this.Victories == 7
                        || this.Defeats + this.Disconnects == 3; 
                case DeckType.SoloArena:
                    return
                        this.Victories == 9
                        || this.Defeats + this.Disconnects == 3;
                default:
                    throw new NotImplementedException("Is arena run finished not dfined for type {" + Type + "}");
            }
        }
    }
}