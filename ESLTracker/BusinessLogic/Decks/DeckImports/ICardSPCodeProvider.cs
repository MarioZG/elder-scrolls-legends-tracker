namespace ESLTracker.BusinessLogic.Decks.DeckImports
{
    public interface ICardSPCodeProvider
    {
        string GetCardByCode(string code);
        string GetCodeByCardName(string cardName);
    }
}