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
    [SerializableVersion(1,0)]
    public class Update_1_0_To_1_1 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(1, 1);

        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            foreach (XmlNode dataNode in doc.SelectNodes("/Tracker/Decks/Deck/ArenaRank"))
            {
                if (!String.IsNullOrWhiteSpace(dataNode.InnerText))
                {
                    ArenaRank value = (ArenaRank)Enum.Parse(typeof(ArenaRank), dataNode.InnerText);
                    dataNode.InnerText = ((int)value).ToString();
                }
            }
        }
    }
}
