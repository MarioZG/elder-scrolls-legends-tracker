using TESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.Utils;

namespace ESLTrackerTests.Builders
{
    public class DeckVersionBuilder
    {
        DeckVersion deckVersion;

        public DeckVersionBuilder()
        {
            deckVersion = new DeckVersion();
            deckVersion.VersionId = Guid.NewGuid();
        }

        public DeckVersionBuilder WithVersion(int major = 0, int minor = 0)
        {
            deckVersion.Version = new SerializableVersion(major, minor);
            return this;
        }

        internal DeckVersionBuilder WithDate(DateTime dateTime)
        {
            deckVersion.CreatedDate = dateTime;
            return this;
        }

        internal DeckVersionBuilder WithCards(IEnumerable<CardInstance> cards)
        {
            foreach (CardInstance c in cards)
            {
                deckVersion.Cards.Add(c);
            }
            return this;
        }

        public DeckVersion Build()
        {
            return deckVersion;
        }


    }
}
