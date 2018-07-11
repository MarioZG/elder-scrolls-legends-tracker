using ESLTracker.Services;
using System;

namespace ESLTracker.BusinessLogic.Cards
{
    public interface ICardsDatabaseFactory
    {
        ICardsDatabase GetCardsDatabase();
        ICardsDatabase RealoadDB();
        ICardsDatabase UpdateCardsDB(string newContent, Version currentVersion);
    }
}