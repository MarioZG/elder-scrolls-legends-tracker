using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.Utils;
using ESLTracker.ViewModels;

namespace ESLTracker.ViewModels.Packs
{
    public class OpenPackViewModel : ViewModelBase
    {
        private Pack pack;

        public Pack Pack
        {
            get { return pack; }
            set {
                pack = value;
                pack.PropertyChanged += Pack_PropertyChanged;
                RaisePropertyChangedEvent(nameof(Pack));
            }
        }

        private void Pack_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(pack.CardSet))
            {

                RaisePropertyChangedEvent(nameof(CardNamesList));
                Pack.Cards = new ObservableCollection<CardInstance>(new List<CardInstance>()
                    { new CardInstance(), new CardInstance(), new CardInstance(),
                      new CardInstance(), new CardInstance(), new CardInstance()});
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        public IEnumerable<string> CardNamesList
        {
            get
            {
                return cardsDatabase.GetCardsNames(pack?.CardSet?.Name);
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; RaisePropertyChangedEvent(nameof(ErrorMessage)); }
        }

        public IAsyncCommand CommandSave { get; private set; }

        private string buttonSaveLabel = "Save";
        public string ButtonSaveLabel
        {
            get { return buttonSaveLabel; }
            set { buttonSaveLabel = value; RaisePropertyChangedEvent(nameof(ButtonSaveLabel)); }
        }

        private bool buttonSaveEnabled = true;
        public bool ButtonSaveEnabled
        {
            get { return buttonSaveEnabled; }
            set { buttonSaveEnabled = value; RaisePropertyChangedEvent(nameof(ButtonSaveEnabled)); }
        }

        public IEnumerable<CardSet> PackSetAutocomplete
        {
            get { return cardsDatabase.CardSets.Where(cs => cs.HasPacks).ToList(); }
        }

        private TrackerFactory trackerFactory;
        ISettings settings;
        ICardsDatabase cardsDatabase;

        public OpenPackViewModel() : this(new TrackerFactory())
        {
            
        }

        public OpenPackViewModel(TrackerFactory trackerFactory)
        {
            CommandSave = new RealyAsyncCommand<object>(CommandSaveExecute);

            this.trackerFactory = trackerFactory;
            settings = trackerFactory.GetService<ISettings>();
            cardsDatabase = trackerFactory.GetService<ICardsDatabase>();

            InitNewPack();
        }

        private void InitNewPack()
        {
            Pack = new Pack(new List<CardInstance>()
                    { new CardInstance(), new CardInstance(), new CardInstance(),
                      new CardInstance(), new CardInstance(), new CardInstance()},
                    true);
            Pack.CardSet = GetDefaultPackSet();
        }


        private async Task<object> CommandSaveExecute(object obj)
        {
            if (pack.Cards.Any(c=> c.Card == null || c.Card == Card.Unknown))
            {
                ErrorMessage = "Please select 6 cards";
                return null;
            }

            ButtonSaveLabel = "Saving...";
            ButtonSaveEnabled = false;

            ErrorMessage = null;

            if (settings.Packs_ScreenshotAfterAdded)
            {
                ButtonSaveLabel = "Taking screenshot...";
                await Task.Factory.StartNew( () => TakePackScreenshot());
            }
            ButtonSaveLabel = "Saving pack...";
            ITracker tracker = trackerFactory.GetTracker();
            Pack.DateOpened = trackerFactory.GetDateTimeNow();
            tracker.Packs.Add(Pack);
            await Task.Factory.StartNew(() => trackerFactory.GetFileManager().SaveDatabase());
            settings.Packs_LastOpenedPackSetId = Pack.CardSet.Id;
            settings.Save();
            InitNewPack();

            ButtonSaveLabel = "Save";
            ButtonSaveEnabled = true;

            return null;
        }

        public async Task TakePackScreenshot()
        {
            string fileName = new ScreenshotNameProvider().GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Pack);
            await trackerFactory.GetFileManager().SaveScreenShot(fileName);
        }

        private CardSet GetDefaultPackSet()
        {
            return cardsDatabase.FindCardSetById(settings.Packs_LastOpenedPackSetId);
        }
    }
}
