using System;
using System.Reflection;

namespace ESLTracker.Utils
{
    public interface IResources
    {
        bool ResourceExists(Uri uri);
        bool ResourceExists(string resourcePath);
        bool ResourceExists(Assembly assembly, string resourcePath);
    }
}