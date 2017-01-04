using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ESLTracker.DataModel;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(1, 1)]
    public class Update_1_2_To_2_0 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(2, 0);

        protected override void VersionSpecificUpdateFile(XmlDocument doc, FileManager fileManager)
        {
            Tracker tracker = fileManager.LoadDatabase();
            CreateInitalHistoryForExistingDecks(tracker);
        }

        internal void CreateInitalHistoryForExistingDecks(Tracker tracker)
        {
            foreach (Deck deck in tracker.Decks)
            {
                if (deck.DoNotUse == null)
                {
                    deck.DoNotUse = new System.Collections.ObjectModel.ObservableCollection<DeckVersion>();
                }
                if (deck.DoNotUse.Count == 0)
                { 
                    deck.CreateVersion(1, 0, deck.CreatedDate);
                }
            }
        }
    }
}
