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
        StringBuilder sbErrors { get; }
        List<CardInstance> Cards { get; }

        void CancelImport();
        Task ImportFromText(string importData);
        void ImportFinished(TaskCompletionSource<bool> tcs);
        Task ImportFromWeb(string webDeckUrl);
    }
}
