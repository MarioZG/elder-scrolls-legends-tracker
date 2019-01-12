using ESLTracker.Utils;
using TESLTracker.Utils;

namespace ESLTracker.BusinessLogic.General
{
    public interface IApplicationInfo
    {
        string GetAssemblyInformationalVersion();
        SerializableVersion GetAssemblyVersion();
        string GetAssemblyFullSemVer();
    }
}