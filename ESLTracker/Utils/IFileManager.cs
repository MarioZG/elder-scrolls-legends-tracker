using ESLTracker.DataModel;

namespace ESLTracker.Utils
{
    public interface IFileManager
    {
        void SaveDatabase();
        Tracker LoadDatabase(bool throwDataFileException = false);
        void SaveScreenShot(string fileName);
    }
}