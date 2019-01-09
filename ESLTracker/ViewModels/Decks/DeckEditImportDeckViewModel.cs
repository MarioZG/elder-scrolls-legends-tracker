using ESLTracker.BusinessLogic.Decks;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.ViewModels.Decks.DeckImports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckEditImportDeckViewModel : ViewModelBase, IDeckEditImportDeckViewModel
    {
        //deckiMporter last chosen from menu, should be set to null when finished
        public IDeckImporter SelectedDeckImporter { get; private set; }
        private DeckEditViewModel parentVM { get; set; }

        private readonly ILogger logger;
        private readonly IEnumerable<IDeckImporter> deckImporters;
        private readonly ImportUIOptionsFactory importUIOptionsFactory;

        private IImportUIOptions importUIOptions;
        public IImportUIOptions ImportUIOptions
        {
            get { return importUIOptions; }
            private set { SetProperty(ref importUIOptions, value); }
        }
        public bool IsDeltaImport { get; set; }


        private string importData;
        public string ImportData
        {
            get { return importData; }
            set { SetProperty<string>(ref importData, value); }
        }

        private string importError;
        public string ImportError
        {
            get { return importError; }
            set { SetProperty<string>(ref importError, value); }
        }

        public ICommand CommandCancel { get; private set; }
        public IAsyncCommand<object> CommandImport { get; private set; }

        public DeckEditImportDeckViewModel(
            ILogger logger,
            IEnumerable<IDeckImporter> deckImporters,
            ImportUIOptionsFactory importUIOptionsFactory
            )
        {
            this.logger = logger;
            this.deckImporters = deckImporters;
            this.importUIOptionsFactory = importUIOptionsFactory;

            CommandCancel = new RelayCommand(CommandCancellExecute);
            CommandImport = new RealyAsyncCommand<object>(CommandImportExecute, CommandImportCanExecute);
        }

        public void SetCurrentImporter(Type type, DeckEditViewModel parentVM, bool getDataFromClipboard = false)
        {
            this.parentVM = parentVM;
            if (type == null)
            {
                SelectedDeckImporter = null;
                ImportUIOptions = null;
                ImportData = null;
            }
            else
            {
                SelectedDeckImporter = deckImporters.Where(di => di.GetType() == type).First();
                ImportUIOptions = importUIOptionsFactory.CreateOptionsFor(type);
                ImportError = null;

                if (getDataFromClipboard)
                {
                    var clipboardContent = Clipboard.GetText();
                    if (SelectedDeckImporter.ValidateInput(clipboardContent))
                    {
                        ImportData = clipboardContent;
                    }
                }
            }
        }

        private void CommandCancellExecute(object obj)
        {
            SelectedDeckImporter?.CancelImport();
            SetCurrentImporter(null, parentVM);
            parentVM.ShowImportPanel = false;
        }

        private async Task<object> CommandImportExecute(object arg)
        {
            logger.Debug($"CommandImportWebExecute {CommandImport.Execution}");
            await Import();
            return true; //any
        }

        private async Task Import()
        {
            logger.Debug($"ImportFromWeb started. Task.IsNotCompleted={CommandImport.Execution?.IsNotCompleted}");
            try
            {
                var result = await SelectedDeckImporter.Import(ImportData, parentVM.Deck);
                logger.Debug($"ImportFromWeb done. tcs={result}; Errors={SelectedDeckImporter.Errors.ToString()}");
                if (result)
                {
                    bool hasWarnings = SelectedDeckImporter.Errors.Length > 0;
                    parentVM.RefreshAfterImport(hasWarnings);
                    if (!hasWarnings)
                    {
                        SetCurrentImporter(null, parentVM);
                    }
                    else
                    {
                        ImportError = "Import warnings:"
                            + Environment.NewLine
                            + SelectedDeckImporter.Errors.ToString();
                    }
                }
                else
                {
                    ImportError = SelectedDeckImporter.Errors.ToString();
                }
            }
            catch (Exception ex)
            {
                ImportError = ex.Message;
            }
        }

        private bool CommandImportCanExecute(object arg)
        {
            if (!String.IsNullOrWhiteSpace(ImportData))
            {
                return SelectedDeckImporter != null && SelectedDeckImporter.ValidateInput(ImportData);
            }
            return false;
        }
    }
}
