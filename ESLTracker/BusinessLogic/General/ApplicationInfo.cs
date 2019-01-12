using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.Utils;

namespace ESLTracker.BusinessLogic.General
{
    public class ApplicationInfo : IApplicationInfo
    {
        public SerializableVersion GetAssemblyVersion()
        {
            return new SerializableVersion(new Version(GitVersionInformation.AssemblySemVer));
        }

        public string GetAssemblyInformationalVersion()
        {
            return string.Format("{0} ({1})", GitVersionInformation.FullSemVer, GitVersionInformation.Sha);
        }

        public string GetAssemblyFullSemVer()
        {
            return GitVersionInformation.FullSemVer;
        }
    }
}
