using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.General;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Services;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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
                Pack.Cards = new ObservableCollection<CardInstance>(cardInstanceFactory.CreateEmptyPack());
                Pack.SetUpChangeEvents();
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

        public IAsyncCommand<object> CommandSave { get; private set; }

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

        ICardInstanceFactory cardInstanceFactory;
        ISettings settings;
        ICardsDatabase cardsDatabase;
        ScreenShot screenShot;
        ITracker tracker;
        IDateTimeProvider dateTimeProvider;
        IFileManager fileManager;
        ScreenshotNameProvider screenshotNameProvider;

        public OpenPackViewModel(
            ICardInstanceFactory cardInstanceFactory,
            ICardsDatabase cardsDatabase,
            ISettings settings,
            ITracker tracker,
            IDateTimeProvider dateTimeProvider,
            IFileManager fileManager,
            ScreenShot screenShot,
            ScreenshotNameProvider screenshotNameProvider)
        {
            this.cardInstanceFactory = cardInstanceFactory;
            this.screenShot = screenShot;
            CommandSave = new RealyAsyncCommand<object>(CommandSaveExecute);

            this.settings = settings;
            this.cardsDatabase = cardsDatabase;
            this.tracker = tracker;
            this.dateTimeProvider = dateTimeProvider;
            this.fileManager = fileManager;
            this.screenshotNameProvider = screenshotNameProvider;


            InitNewPack();
        }

        private void InitNewPack()
        {
            Pack = new Pack(cardInstanceFactory.CreateEmptyPack(), true);
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
            Pack.DateOpened = dateTimeProvider.DateTimeNow;
            tracker.Packs.Add(Pack);
            await Task.Factory.StartNew(() => fileManager.SaveDatabase());
            settings.Packs_LastOpenedPackSetId = Pack.CardSet.Id;
            settings.Save();
            InitNewPack();

            ButtonSaveLabel = "Save";
            ButtonSaveEnabled = true;

            return null;
        }

        public async Task TakePackScreenshot()
        {
            string fileName = screenshotNameProvider.GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Pack);
            await screenShot.SaveScreenShot(fileName);
        }

        private CardSet GetDefaultPackSet()
        {
            return cardsDatabase.FindCardSetById(settings.Packs_LastOpenedPackSetId);
        }
    }
}
