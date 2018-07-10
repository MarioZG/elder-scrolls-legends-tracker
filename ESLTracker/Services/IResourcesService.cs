using System;
using System.Reflection;

namespace ESLTracker.Services
{
    public interface IResourcesService
    {
        bool ResourceExists(Uri uri);
        bool ResourceExists(string resourcePath);
        bool ResourceExists(Assembly assembly, string resourcePath);
    }
}