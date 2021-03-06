﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ESLTracker.BusinessLogic.Cards;
using TESLTracker.DataModel;
using ESLTracker.Utils.Extensions;
using TESLTracker.Utils;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(2, 3)]
    public class Update_2_3_To_3_0 : UpdateBase
    {
        private readonly ICardsDatabase cardsDatabase;

        public Update_2_3_To_3_0(ILogger logger, ICardsDatabase cardsDatabase) : base(logger)
        {
            this.cardsDatabase = cardsDatabase;
        }

        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(3, 0);

        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            logger.Info("Start file conversion to {0}", TargetVersion);
            SetPacksToCore(tracker);
            doc.InnerXml = SerializationHelper.SerializeXML(tracker);
            logger.Info("Finished file conversion to {0}", TargetVersion);
        }

        public void SetPacksToCore(ITracker tracker)
        {
            CardSet core = cardsDatabase.CardSets.Where(cs => cs.Name == "Core").Single();
            foreach (Pack p in tracker.Packs)
            {
                p.CardSet = core;
            }
            logger.Info("Converted {0} packs", tracker.Packs.Count);
        }

    }
}
