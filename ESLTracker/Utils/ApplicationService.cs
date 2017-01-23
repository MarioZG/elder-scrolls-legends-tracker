using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class ApplicationService : IApplicationService
    {
        public SerializableVersion GetAssemblyVersion()
        {
            string versionString = Assembly.GetEntryAssembly().CustomAttributes.Where(ca => ca.AttributeType == typeof(AssemblyFileVersionAttribute)).FirstOrDefault()?.ConstructorArguments?.FirstOrDefault().Value.ToString();
            return new SerializableVersion(new Version(versionString));
        }

        public SerializableVersion GetAssemblyInformationalVersion()
        {
            return
                 new SerializableVersion(new Version(
                     String.Join(";", Assembly.GetEntryAssembly().CustomAttributes.Where(ca => ca.AttributeType == typeof(AssemblyInformationalVersionAttribute)).FirstOrDefault()?.ConstructorArguments)
                 ));
        }
    }
}
