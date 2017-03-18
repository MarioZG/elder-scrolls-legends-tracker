using ESLTracker.Utils;

namespace ESLTracker.Services
{
    public interface IApplicationService
    {
        string GetAssemblyInformationalVersion();
        SerializableVersion GetAssemblyVersion();
    }
}