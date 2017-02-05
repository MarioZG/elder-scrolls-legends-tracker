namespace ESLTracker.Utils
{
    public interface IApplicationService
    {
        string GetAssemblyInformationalVersion();
        SerializableVersion GetAssemblyVersion();
    }
}