using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.Utils;

namespace ESLTracker.Services
{
    public class ApplicationService : IApplicationService
    {
        public SerializableVersion GetAssemblyVersion()
        {
            return new SerializableVersion(new Version(GitVersionInformation.AssemblySemVer));
        }

        public string GetAssemblyInformationalVersion()
        {
            return string.Format("{0} ({1})", GitVersionInformation.FullSemVer, GitVersionInformation.Sha);
        }
    }
}
