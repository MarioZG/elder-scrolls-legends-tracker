using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(1,1)]
    public class Update_1_1_To_1_2 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(1, 2);

        public Update_1_1_To_1_2(ILogger logger) : base(logger)
        {

        }

        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            //do nothing - just marking packs changes
        }
    }
}
