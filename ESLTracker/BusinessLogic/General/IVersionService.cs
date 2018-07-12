using ESLTracker.BusinessLogic.Cards;
using ESLTracker.Utils;

namespace ESLTracker.BusinessLogic.General
{
    public interface IVersionService
    {
        NewVersioInfo AppVersionInfo { get; }

        NewVersioInfo CheckNewAppVersionAvailable();
        ICardsDatabase GetLatestCardsDB();
        bool IsNewCardsDBAvailable();
    }
}