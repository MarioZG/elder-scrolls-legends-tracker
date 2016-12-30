using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    /// <summary>
    /// http://stackoverflow.com/questions/2013481/detect-whether-wpf-resource-exists-based-on-uri
    /// </summary>
    class ResourcesHelper
    {
        public static bool ResourceExists(Uri uri)
        {
            string path = String.Join("", uri.Segments.Skip(1)); //segments[0] = '/' - we need to skip this
            return ResourceExists(path);
        }
        public static bool ResourceExists(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return ResourceExists(assembly, resourcePath);
        }

        public static bool ResourceExists(Assembly assembly, string resourcePath)
        {
            return GetResourcePaths(assembly)
                .Contains(resourcePath.ToLowerInvariant());
        }

        public static IEnumerable<object> GetResourcePaths(Assembly assembly)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var resourceName = assembly.GetName().Name + ".g";
            var resourceManager = new ResourceManager(resourceName, assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                foreach (System.Collections.DictionaryEntry resource in resourceSet)
                {
                    yield return resource.Key;
                }
            }
            finally
            {
                resourceManager.ReleaseAllResources();
            }
        }
    }
}
