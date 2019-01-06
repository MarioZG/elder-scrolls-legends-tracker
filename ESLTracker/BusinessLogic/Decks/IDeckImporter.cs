using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Decks
{
    public interface IDeckImporter
    {
        string DeckName { get; }
        StringBuilder Errors { get; }
        List<CardInstance> Cards { get; }

        bool ValidateInput(object data);
        void CancelImport();
        Task<bool> Import(object data, Deck deck = null);
    }
}
