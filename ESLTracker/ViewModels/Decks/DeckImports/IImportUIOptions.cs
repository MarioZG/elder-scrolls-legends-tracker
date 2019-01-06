using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Decks.DeckImports
{
    public interface IImportUIOptions
    {
        string Prompt { get; }
        bool IsInputMultiLine { get; }
        bool ShowDeltaOption { get; }
    }
}
