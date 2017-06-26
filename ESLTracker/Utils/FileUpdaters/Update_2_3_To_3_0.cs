using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ESLTracker.DataModel;
using ESLTracker.Services;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(2, 3)]
    public class Update_2_3_To_3_0 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(3, 0);

        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            Logger.Info("Start file conversion to {0}", TargetVersion);
            SetPacksToCore(tracker);
            doc.InnerXml = SerializationHelper.SerializeXML(tracker);
            Logger.Info("Finished file conversion to {0}", TargetVersion);
        }

        public void SetPacksToCore(ITracker tracker)
        {
            ICardsDatabase cardsDb = TrackerFactory.DefaultTrackerFactory.GetService<ICardsDatabase>();
            CardSet core = cardsDb.CardSets.Where(cs => cs.Name == "Core").Single();
            foreach (Pack p in tracker.Packs)
            {
                p.CardSet = core;
            }
            Logger.Info("Converted {0} packs", tracker.Packs.Count);
        }

    }
}
