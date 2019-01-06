using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Decks.DeckImports
{
    abstract public class BaseImporter : IDeckImporter
    {
        //In
        protected object ImportData { get; set; }

        //out
        public StringBuilder Errors { get; } = new StringBuilder();
        public string Status { get; set; }
        public List<CardInstance> Cards { get; set; } = new List<CardInstance>();
        public string DeckName { get; protected set; }

        private bool wasCancelled = false;

        protected readonly ICardsDatabaseFactory cardsDatabaseFactory;
        protected readonly ICardInstanceFactory cardInstanceFactory;

        public BaseImporter(ICardsDatabaseFactory cardsDatabaseFactory, ICardInstanceFactory cardInstanceFactory)
        {
            this.cardsDatabaseFactory = cardsDatabaseFactory;
            this.cardInstanceFactory = cardInstanceFactory;
        }

        public async Task<bool> Import(object importData, Deck deck = null)
        {
            ImportData = importData;

            Cards.Clear();
            Errors.Clear();

            try
            {
                await Task.Run(() => ExecuteImport(importData));
                await Task.Run(() => ExecutePostImportFixUp(deck));

                return ! wasCancelled;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CancelImport()
        {
            wasCancelled = true;
        }

        internal abstract void ExecuteImport(object importData);

        internal virtual void ExecutePostImportFixUp(Deck deck)
        {
            if (!String.IsNullOrWhiteSpace(DeckName))
            {
                deck.Name = DeckName;
            }

            deck.SelectedVersion.Cards = new PropertiesObservableCollection<CardInstance>(Cards);
            deck.Class = ClassAttributesHelper.FindSingleClassByAttribute(Cards.SelectMany(c => c.Card.Attributes).Distinct());

        }

        public abstract bool ValidateInput(object data);
    }
}
