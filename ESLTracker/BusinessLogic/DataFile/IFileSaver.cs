using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.DataFile
{
    public interface IFileSaver
    {
        void SaveDatabase(ITracker tracker);
       
    }
}