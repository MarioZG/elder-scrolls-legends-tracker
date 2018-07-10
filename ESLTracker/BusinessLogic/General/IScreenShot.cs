using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.General
{
    public interface IScreenShot
    {
        Task SaveScreenShot(string fileName);
    }
}