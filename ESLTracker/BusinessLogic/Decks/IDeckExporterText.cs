using ESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Decks
{
    public interface IDeckExporterText
    {
        bool ExportDeck(Deck deck);
    }
}