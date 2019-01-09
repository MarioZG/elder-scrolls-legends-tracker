using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TESLTracker.DataModel;
using TESLTracker.Utils;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(2, 1)]
    public class Update_2_1_To_2_2 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(2, 2);

        public Update_2_1_To_2_2(ILogger logger) : base(logger)
        {

        }

        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            //do nothing
        }

    }
}
