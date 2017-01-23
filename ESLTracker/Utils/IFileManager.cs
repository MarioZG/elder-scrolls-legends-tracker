using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.Utils
{
    public interface IFileManager
    {
        void SaveDatabase();
        Tracker LoadDatabase(bool throwDataFileException = false);
        Task SaveScreenShot(string fileName);
        void UpdateCardsDB(string newContent);
    }
}