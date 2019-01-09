using System.Threading.Tasks;
using TESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.DataFile
{
    public interface IFileSaver
    {
        void SaveDatabase(ITracker tracker);
       
    }
}