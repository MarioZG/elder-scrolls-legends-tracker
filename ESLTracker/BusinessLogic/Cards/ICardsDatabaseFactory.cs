using ESLTracker.Services;

namespace ESLTracker.BusinessLogic.Cards
{
    public interface ICardsDatabaseFactory
    {
        ICardsDatabase GetCardsDatabase();
        ICardsDatabase RealoadDB();
    }
}