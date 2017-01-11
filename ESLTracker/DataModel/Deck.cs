using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;

namespace ESLTracker.DataModel
{
    public class Deck : ViewModels.ViewModelBase, ICloneable, IEquatable<Deck>
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
                    return new DeckAttributes(Class.Value, ClassAttributesHelper.Classes[Class.Value]);
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

        
        [XmlIgnore]
        public ReadOnlyCollection<DeckVersion> History
        {
            get
            {
                return DoNotUse.OrderBy( dv => dv.Version).ToList().AsReadOnly();
            }
        }


        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //obsolete prevents serialisation!!!!
        //[Obsolete("This is only for serialization process", true)]
        [XmlArray("DeckVersions")]
        public ObservableCollection<DeckVersion> DoNotUse { get; set; } = new ObservableCollection<DeckVersion>();

        private Guid selectedVersionId;
        public Guid SelectedVersionId
        {
            get
            {
                return selectedVersionId != null ? selectedVersionId : History.OrderByDescending( dv => dv.Version).First().VersionId ;
            }
            set
            {
                selectedVersionId = value;
            }
        }

        [XmlIgnore]
        public DeckVersion SelectedVersion
        {
            get
            {
                return this.History.Where( dh => dh.VersionId == this.selectedVersionId).FirstOrDefault();
            }
        }

        private ITrackerFactory trackerFactory; //cannot be ITracker, as we need to load it first - stack overflow when database is loading

        [Obsolete("Use static CreateNewDeck instead. This is public  if for serialization purpose only")]
        public Deck() : this(TrackerFactory.DefaultTrackerFactory)
        {
        }

        [Obsolete("Use static CreateNewDeck instead. This is public  if for serialization purpose only")]
        internal Deck(ITrackerFactory tracker)
        {
            DeckId = tracker.GetNewGuid(); //if deserialise, will be overriten!, if new generate!
            DateTime createdDateTime = tracker.GetDateTimeNow();
            CreatedDate = createdDateTime;
            this.trackerFactory = tracker;
        }

        internal static Deck CreateNewDeck()
        {
            return CreateNewDeck(TrackerFactory.DefaultTrackerFactory);
        }

        internal static Deck CreateNewDeck(string deckName)
        {
            return CreateNewDeck(TrackerFactory.DefaultTrackerFactory, deckName);
        }

        public static Deck CreateNewDeck(ITrackerFactory trackerFactory, string deckName = "")
        {
            Deck deck = new Deck(trackerFactory);
            deck.CreateVersion(1, 0, trackerFactory.GetDateTimeNow());
            deck.Name = deckName;
            return deck;
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
            return trackerFactory.GetTracker().Games.Where(g => g.Deck.DeckId == this.DeckId);
        }

        public dynamic GetDeckVsClass()
        {
            return GetDeckVsClass(null);
        }

        public dynamic GetDeckVsClass(DeckClass? opponentClass)
        {
            return this.GetDeckGames()
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

        public void UpdateAllBindings()
        {
            RaisePropertyChangedEvent("");
        }

        public object Clone()
        {
            Deck deck = this.MemberwiseClone() as Deck;
            if (deck != null)
            {
                deck.DoNotUse = this.DoNotUse.DeepCopy<DeckVersion>();
            }
            return deck;
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

        public IEnumerable<Reward> GetArenaRewards()
        {
            return trackerFactory.GetTracker().Rewards
                .Where(r=> r.ArenaDeckId == DeckId);
        }


        /// <summary>
        /// Creates new deck version in history, adds to colletion and returns reference
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        public DeckVersion CreateVersion(int major, int minor, DateTime createdDate)
        {
            SerializableVersion version = new SerializableVersion(major, minor);
            if (DoNotUse.Any(v => v.Version == version))
            {
                throw new ArgumentException(string.Format("Version {0} alread has been added to deck '{1}'", version, Name));
            }
            DeckVersion dv = new DeckVersion();
            dv.CreatedDate = createdDate;
            dv.Version = version;
            this.DoNotUse.Add(dv); //add to history
            this.SelectedVersionId = dv.VersionId;
            return dv;
        }

        /// <summary>
        /// Apply history from other deck instance. used in cancel edit on view model
        /// </summary>
        /// <param name="savedState"></param>
        internal void CopyHistory(IEnumerable<DeckVersion> history)
        {
            if ((history == null) || (history.Count() == 0))
            {
                throw new ArgumentException("History cannot be null or empty");
            }
            this.DoNotUse = new ObservableCollection<DeckVersion>(history);
            if (! this.History.Any( dv => dv.VersionId == this.SelectedVersionId))
            {
                this.SelectedVersionId = this.History.OrderByDescending(dv => dv.Version).First().VersionId;
            }
        }

        public override int GetHashCode()
        {
            return DeckId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Deck);
        }

        public bool Equals(Deck other)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
                return false;

            bool equals = true;

            equals &= this.ArenaRank == other.ArenaRank;
            //equals &= this.Attributes == other.Attributes; //attributes depends only on class - 
            equals &= this.Class == other.Class;
            equals &= this.CreatedDate == other.CreatedDate;
            equals &= this.DeckId == other.DeckId;
            equals &= Enumerable.SequenceEqual(this.History, other.History);
            equals &= this.Name == other.Name;
            equals &= this.Notes == other.Notes;
            equals &= this.SelectedVersionId == other.SelectedVersionId;
            equals &= this.Type == other.Type;

            return equals;
        }

        public static bool operator ==(Deck lhs, Deck rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Deck lhs, Deck rhs)
        {
            return !(lhs == rhs);
        }


    }
}