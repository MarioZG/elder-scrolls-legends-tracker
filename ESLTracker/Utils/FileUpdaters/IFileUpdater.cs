using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;
using TESLTracker.Utils;

namespace ESLTracker.Utils.FileUpdaters
{
    public interface IFileUpdater
    {
        /// <summary>
        /// target (final) version of update
        /// </summary>
        SerializableVersion TargetVersion { get; }

        /// <summary>
        /// Updates file to specific version
        /// </summary>
        /// <param name="filePath">file to update</param>
        /// <param name="tracker">if loaded succesfully, current version of tracker. if exception encountered and canot be loaded, null, </param>
        /// <returns></returns>
        bool UpdateFile(string filePath, Tracker tracker);
    }
}
