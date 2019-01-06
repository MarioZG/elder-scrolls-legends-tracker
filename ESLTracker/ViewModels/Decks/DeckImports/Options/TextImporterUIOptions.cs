using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Decks.DeckImports.Options
{
    public class TextImporterUIOptions : IImportUIOptions
    {
        public string Prompt => 
@"Provide decklist. One card in each line, first quantity of card followed by space and card name. Example:

3 Swamp Leviathan
2 Hist Groove

If using delta import, cards to remove provide as negative quantity

-2 Calm
2 Execute

will remove two calm cards and add two execute cards to current deck";

        public bool IsInputMultiLine => true;

        public bool ShowDeltaOption => true;
    }
}
