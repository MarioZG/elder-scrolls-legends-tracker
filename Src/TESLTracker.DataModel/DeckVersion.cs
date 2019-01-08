using System;
using System.Diagnostics;
using System.Linq;
using TESLTracker.Utils;

namespace TESLTracker.DataModel
{
    [DebuggerDisplay("Version={Version}; CardsCount={Cards.Count}")]
    public class DeckVersion: IEquatable<DeckVersion>, ICloneable
    {
        public Guid VersionId { get; set; } 

        public SerializableVersion Version { get; set; }
        public PropertiesObservableCollection<CardInstance> Cards { get; set; } = new PropertiesObservableCollection<CardInstance>();

        public DateTime CreatedDate { get; set; }


        public override int GetHashCode()
        {
            return VersionId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DeckVersion);
        }

        public bool Equals(DeckVersion other)
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

            equals &= this.CreatedDate == other.CreatedDate;
            equals &= this.Version == other.Version;
            equals &= this.VersionId == other.VersionId;
            equals &= Enumerable.SequenceEqual(this.Cards, other.Cards);

            return equals;
        }

        public static bool operator ==(DeckVersion lhs, DeckVersion rhs)
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

        public static bool operator !=(DeckVersion lhs, DeckVersion rhs)
        {
            return !(lhs == rhs);
        }

        public object Clone()
        {
            DeckVersion dv = this.MemberwiseClone() as DeckVersion;
            if (dv != null)
            {
                dv.Cards = this.Cards?.DeepCopy<PropertiesObservableCollection<CardInstance>, CardInstance>();
                dv.Version = this.Version?.Clone() as SerializableVersion;
            }
            return dv;
        }
    }
}
