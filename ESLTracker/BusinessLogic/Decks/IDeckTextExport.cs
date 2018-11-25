using ESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Decks
{
    public interface IDeckTextExport
    {
        string FormatCardLine(CardInstance card);
        string FormatDeckHeader(Deck deck);
        string FormatDeckFooter(Deck deck);
    }
}