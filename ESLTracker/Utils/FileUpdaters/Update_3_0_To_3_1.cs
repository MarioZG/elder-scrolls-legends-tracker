using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(3,0)]
    public class Update_3_0_To_3_1 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(3, 1);



        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            Logger.Info("Start file conversion to {0}", TargetVersion);
            UpdateCardsGuidInDecks(doc);
            // decks (hist)
            // packs
            // rewards
            Logger.Info("Finished file conversion to {0}", TargetVersion);
        }

        public void UpdateCardsGuidInDecks(XmlDocument doc)
        {
            foreach (var translate in CardsDatabase.GuidTranslation)
            {
                Logger.Info($"Processing chage from {translate.Key} to {translate.Value}");

                var nodes = doc.SelectNodes($"*//CardInstance/CardId[text()='{translate.Key}']");
                Logger.Info($"{nodes.Count} occurences found");

                foreach (XmlNode node in nodes)
                {
                    node.InnerText = translate.Value;
                }
                Logger.Info($"Finished");
            }
        }

    }
}
