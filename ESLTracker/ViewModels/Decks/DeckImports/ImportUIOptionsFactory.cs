using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Decks.DeckImports;
using ESLTracker.ViewModels.Decks.DeckImports.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Decks.DeckImports
{
    public class ImportUIOptionsFactory
    {

        public IImportUIOptions CreateOptionsFor(Type type)
        {
            switch (type.Name)
            {
                case nameof(SPCodeImporter):
                    return new SPCodeImporterUIOptions();
                case nameof(TextImporter):
                    return new TextImporterUIOptions();
                case nameof(WebImporter):
                    return new WebImporterUIOptions();
                default:
                    throw new NotImplementedException($"Unknown deck importer type: {type.Name}");
            }

        }
    }
}
