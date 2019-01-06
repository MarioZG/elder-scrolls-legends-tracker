using System;
using ESLTracker.BusinessLogic.Decks;

namespace ESLTracker.ViewModels.Decks
{
    public interface IDeckEditImportDeckViewModel
    {
        IDeckImporter SelectedDeckImporter { get; }

        void SetCurrentImporter(Type type, DeckEditViewModel parentVM, bool getDataFromClipboard = false);
    }
}