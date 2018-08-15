using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;

namespace ESLTracker.Utils.FileUpdaters
{
    [SerializableVersion(1, 2)]
    public class Update_1_2_To_2_0 : UpdateBase
    {
        public override SerializableVersion TargetVersion { get; } = new SerializableVersion(2, 0);

        IDeckService deckService;

        public Update_1_2_To_2_0(ILogger logger, IDeckService deckService) : base(logger)
        {
            this.deckService = deckService;
        }

        protected override void VersionSpecificUpdateFile(XmlDocument doc, Tracker tracker)
        {
            UpdateCardInstanceQtyTo1_OnPacks(tracker);
            UpdateCardInstanceQtyTo1_OnRewards(tracker);
            CreateInitalHistoryForExistingDecks(tracker);
            AllocateGamesToInitalVersionOfDeck(tracker);
            doc.InnerXml = SerializationHelper.SerializeXML(tracker);
        }

        public void CreateInitalHistoryForExistingDecks(Tracker tracker)
        {
            foreach (Deck deck in tracker.Decks)
            {
                if (deck.DoNotUse == null)
                {
                    deck.DoNotUse = new System.Collections.ObjectModel.ObservableCollection<DeckVersion>();
                }
                if (deck.DoNotUse.Count == 0)
                {
                    deckService.CreateDeckVersion(deck, 1, 0, deck.CreatedDate);
                }
            }
        }

        private static void AllocateGamesToInitalVersionOfDeck(Tracker tracker)
        {
            //allocate games to default version
            foreach (Game game in tracker.Games)
            {
                game.DeckVersionId = game.Deck.SelectedVersionId;
            }
        }

        private void UpdateCardInstanceQtyTo1_OnRewards(Tracker tracker)
        {
            foreach(Pack p in tracker.Packs)
            {
                foreach (CardInstance ci in p.Cards)
                {
                    if (ci.Quantity == 0)
                    {
                        ci.Quantity = 1;
                    }
                }
            }
        }

        private void UpdateCardInstanceQtyTo1_OnPacks(Tracker tracker)
        {
            foreach (Reward r in tracker.Rewards.Where( r=> r.Type == DataModel.Enums.RewardType.Card))
            {
                if ((r.CardInstance != null) && (r.CardInstance.Quantity == 0))
                {
                    r.CardInstance.Quantity = 1;
                }
            }
        }
    }
}
