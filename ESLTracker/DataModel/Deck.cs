using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils.Extensions;

namespace ESLTracker.DataModel
{
    [DebuggerDisplay("{DebuggerInfo}")]
    public class Deck : ViewModels.ViewModelBase, ICloneable, IEquatable<Deck>, IComparable
    {
        public Guid DeckId { get; set; }

        private DeckType type;
        public DeckType Type
        {
            get { return type; }
            set { type = value; RaisePropertyChangedEvent("Type"); }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetProperty<string>(ref name, value); }
        }

        private DeckClass? deckClass;
        public DeckClass? Class
        {
            get { return deckClass; }
            set { SetProperty<DeckClass?>(ref deckClass, value); }
        }

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
                RaisePropertyChangedEvent(nameof(SelectedVersion));
                RaisePropertyChangedEvent(nameof(SelectedVersionId));
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

        public string DebuggerInfo
        {
            get
            {
                return string.Format("Name={0};SelectedVersion={1:mm};", Name, SelectedVersion.Version);
            }
        }

        public bool IsHidden { get; set; }

        private DateTime lastUsed = DateTime.MinValue;
        public DateTime LastUsed
        {
            get
            {
                return lastUsed;
            }
            set
            {
                SetProperty<DateTime>(ref lastUsed, value);
            }
        }

        private string deckUrl;

        [XmlAttribute]
        public string DeckUrl
        {
            get { return deckUrl; }
            set { SetProperty<string>(ref deckUrl, value); }
        }

        [XmlIgnore]
        public bool IsWebDeck
        {
            get
            {
                return ! String.IsNullOrWhiteSpace(deckUrl);
            }
        }


        /// <summary>
        /// user tag for deck: aggro, control etc...
        /// </summary>
        public string DeckTag { get; set; }

        [Obsolete("Use factory in production code or deckbuilder in unit tests to create new decks")]
        public Deck()
        {
       
        }

        public object Clone()
        {
            Deck deck = this.MemberwiseClone() as Deck;
            deck.ClearPropertyChanged();
            if (deck != null)
            {
                deck.DoNotUse = this.DoNotUse?.DeepCopy<ObservableCollection<DeckVersion>, DeckVersion>();
            }
            return deck;
        }

        public static bool IsArenaDeck(DeckType type)
        {
            return type == DeckType.SoloArena || type == DeckType.VersusArena;
        }

        /// <summary>
        /// Apply history from other deck instance. used in cancel edit on view model
        /// </summary>
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
            equals &= this.IsHidden == other.IsHidden;
            equals &= this.LastUsed == other.LastUsed;
            equals &= this.DeckTag == other.DeckTag;
            equals &= this.DeckUrl == other.DeckUrl;

            return equals;
        }

        public int CompareTo(object obj)
        {
            if (obj is Deck)
            {
                return string.Compare(Name, ((Deck)obj).Name, StringComparison.InvariantCulture);
            }
            else
            {
                return -1;
            }
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