using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Builders
{
    public class DeckBuilder
    {
        Deck deck;

        public DeckBuilder()
        {
            deck = new Deck(); //TODO: id and created not set
            deck.DeckId = Guid.NewGuid();
            deck.CreatedDate = DateTime.Now;
            deck.Name = "Test deck";
            deck.Type = DeckType.Constructed;
        }

        public DeckBuilder WithType(DeckType type)
        {
            deck.Type = type;
            return this;
        }

        public DeckBuilder WithClass(DeckClass deckClass)
        {
            deck.Class = deckClass;
            return this;
        }

        internal DeckBuilder WithCreatedDate(DateTime createdDate)
        {
            deck.CreatedDate = createdDate;
            return this;
        }

        internal DeckBuilder WithIsHidden(bool isHidden)
        {
            deck.IsHidden = isHidden;
            return this;
        }

        internal DeckBuilder WithName(string name)
        {
            deck.Name = name;
            return this;
        }

        public DeckBuilder WithDefaultVersion()
        {
            DeckVersion dv = new DeckVersionBuilder().WithVersion(1, 0).Build();
            deck.DoNotUse.Add(dv);
            deck.SelectedVersionId = dv.VersionId;
            return this;
        }

        public DeckBuilder WithVersion(DeckVersion deckVersion)
        {
            deck.DoNotUse.Add(deckVersion);
            return this;
        }

        public DeckBuilder WithSelectedVersion(DeckVersion deckVersion)
        {
            deck.DoNotUse.Add(deckVersion);
            deck.SelectedVersionId = deckVersion.VersionId;
            return this;
        }

        public Deck Build()
        {
            return deck;
        }


    }
}
