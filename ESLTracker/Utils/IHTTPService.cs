namespace ESLTracker.Utils
{
    public interface IHTTPService
    {
        string SendGetRequest(string url);
        string SendPostRequest(string url, string postData);
    }
}