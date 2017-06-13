using ESLTracker.Utils;

namespace ESLTracker.Services
{
    public interface IVersionService
    {
        NewVersioInfo AppVersionInfo { get; }

        NewVersioInfo CheckNewAppVersionAvailable();
        ICardsDatabase GetLatestCardsDB();
        bool IsNewCardsDBAvailable();
    }
}