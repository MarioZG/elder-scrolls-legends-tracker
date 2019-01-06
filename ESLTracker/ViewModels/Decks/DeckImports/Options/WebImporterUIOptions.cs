using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Decks.DeckImports.Options
{
    class WebImporterUIOptions : IImportUIOptions
    {
        public string Prompt => "Please provide url";

        public bool IsInputMultiLine => false;

        public bool ShowDeltaOption => false;
    }
}
