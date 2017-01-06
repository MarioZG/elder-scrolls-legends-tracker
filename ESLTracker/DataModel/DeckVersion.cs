using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils;

namespace ESLTracker.DataModel
{
    public class DeckVersion: IEquatable<DeckVersion>
    {
        public Guid VersionId { get; set; } 

        public SerializableVersion Version { get; set; }
        public ObservableCollection<CardInstance> Cards { get; set; } = new ObservableCollection<CardInstance>();

        public DateTime CreatedDate { get; set; }

        private ITrackerFactory trackerFactory;

        public DeckVersion() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public DeckVersion(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            VersionId = trackerFactory.GetNewGuid();
        }

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
    }
}
