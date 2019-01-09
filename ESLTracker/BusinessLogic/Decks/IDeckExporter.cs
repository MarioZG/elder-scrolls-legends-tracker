using TESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Decks
{
    public interface IDeckExporter
    {
        bool ExportDeck(Deck deck);
    }
}