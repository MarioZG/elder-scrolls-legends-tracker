using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ESLTracker.DataModel;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(2, 3)]
    public class Update_2_3_To_3_0 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(3, 0);

        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            //do nothing - just mark for new gametype values
        }

    }
}
