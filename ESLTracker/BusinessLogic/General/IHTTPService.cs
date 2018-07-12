namespace ESLTracker.BusinessLogic.General
{
    public interface IHTTPService
    {
        string SendGetRequest(string url);
        string SendPostRequest(string url, string postData);
    }
}