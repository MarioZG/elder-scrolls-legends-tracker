﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TESLTracker.DataModel;
using TESLTracker.Utils;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(2, 2)]
    public class Update_2_2_To_2_3 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(2, 3);

        public Update_2_2_To_2_3(ILogger logger) : base(logger)
        {

        }

        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            //do nothing - just mark for new gametype values
        }

    }
}
