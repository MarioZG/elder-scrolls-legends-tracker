using TESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Decks.DeckExports
{
    public interface IDeckTextExport
    {
        string FormatCardLine(CardInstance card);
        string FormatDeckHeader(Deck deck);
        string FormatDeckFooter(Deck deck);
    }
}