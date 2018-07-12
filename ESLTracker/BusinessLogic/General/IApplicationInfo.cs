using ESLTracker.Utils;

namespace ESLTracker.BusinessLogic.General
{
    public interface IApplicationInfo
    {
        string GetAssemblyInformationalVersion();
        SerializableVersion GetAssemblyVersion();
    }
}