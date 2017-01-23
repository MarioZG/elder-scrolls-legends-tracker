namespace ESLTracker.Utils
{
    public interface IApplicationService
    {
        SerializableVersion GetAssemblyInformationalVersion();
        SerializableVersion GetAssemblyVersion();
    }
}