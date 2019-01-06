using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Decks.DeckImports.Options
{
    public class SPCodeImporterUIOptions : IImportUIOptions
    {
        public string Prompt => "Paste SP code:";

        public bool IsInputMultiLine =>  false;

        public bool ShowDeltaOption => false;
    }
}
