using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Services;

namespace ESLTracker.BusinessLogic.DataFile
{
    public interface IFileSaver
    {
        void SaveDatabase(ITracker tracker);
       
    }
}