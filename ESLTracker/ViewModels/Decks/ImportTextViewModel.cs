using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.Utils;
using ESLTracker.Services;

namespace ESLTracker.ViewModels.Decks
{
    public class ImportTextViewModel : ViewModelBase
    {

        public string ImportData { get; set; }
        public IDeckImporter DeckImporter { get; set; }

        public ICommand CommandCancel
        {
            get
            {
                return new RelayCommand(CommandCancelExecute);
            }
        }

        public ICommand CommandImport
        {
            get
            {
                return new RealyAsyncCommand<object>(CommandImportExecute);
            }
        }

        private ITrackerFactory trackerFactory;        

        public ImportTextViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public ImportTextViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            DeckImporter = trackerFactory.GetService<IDeckImporter>();
        }

        private void CommandCancelExecute(object obj)
        {
            DeckImporter.CancelImport();
        }

        private async Task<object> CommandImportExecute(object param)
        {
            await DeckImporter.ImportFromText(ImportData); //results of this cass will be handled in deckpreviewdatamodel
            return null;
        }
    }
}
