using ESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Decks
{
    public interface IDeckExporter
    {
        bool ExportDeck(Deck deck);
    }
}