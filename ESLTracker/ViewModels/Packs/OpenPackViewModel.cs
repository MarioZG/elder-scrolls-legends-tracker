using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.General;
using ESLTracker.BusinessLogic.Packs;
using ESLTracker.DataModel;
using ESLTracker.Properties;
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

        public int PackSoulGemsValue
        {
            get
            {
                return soulGemCalculator.CalculateCardsSellValue(Pack.Cards);
            }
        }
           

        private void Pack_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(pack.CardSet))
            {

                RaisePropertyChangedEvent(nameof(CardNamesList));
                packFactory.ClearPack(Pack, this.RefreshBindings);
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        public IEnumerable<string> CardNamesList
        {
            get
            {
                return cardsDatabaseFactory.GetCardsDatabase().GetCardsNames(pack?.CardSet?.Name);
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

        private readonly ICardInstanceFactory cardInstanceFactory;
        private readonly ISettings settings;
        private readonly ICardsDatabaseFactory cardsDatabaseFactory;
        private readonly IScreenShot screenShot;
        private readonly ITracker tracker;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IFileSaver fileManager;
        private readonly ScreenshotNameProvider screenshotNameProvider;
        private readonly PackFactory packFactory;
        private readonly SoulGemCalculator soulGemCalculator;

        public OpenPackViewModel(
            ICardInstanceFactory cardInstanceFactory,
            ICardsDatabaseFactory cardsDatabaseFactory,
            ISettings settings,
            ITracker tracker,
            IDateTimeProvider dateTimeProvider,
            IFileSaver fileManager,
            IScreenShot screenShot,
            ScreenshotNameProvider screenshotNameProvider,
            PackFactory packFactory,
            SoulGemCalculator soulGemCalculator)
        {
            this.cardInstanceFactory = cardInstanceFactory;
            this.screenShot = screenShot;
            CommandSave = new RealyAsyncCommand<object>(CommandSaveExecute);

            this.settings = settings;
            this.cardsDatabaseFactory = cardsDatabaseFactory;
            this.tracker = tracker;
            this.dateTimeProvider = dateTimeProvider;
            this.fileManager = fileManager;
            this.screenshotNameProvider = screenshotNameProvider;
            this.packFactory = packFactory;
            this.soulGemCalculator = soulGemCalculator;

            InitNewPack();
        }

        private void InitNewPack()
        {
            Pack = packFactory.CreateEmptyPack(true, this.RefreshBindings);
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
            await Task.Factory.StartNew(() => fileManager.SaveDatabase(tracker));
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
            return cardsDatabaseFactory.GetCardsDatabase().FindCardSetById(settings.Packs_LastOpenedPackSetId);
        }


        internal void RefreshBindings(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.PropertyName)
                  || (e.PropertyName == nameof(CardInstance.Card))
                  || (e.PropertyName == nameof(CardInstance.IsPremium)))
            {
                RaisePropertyChangedEvent(nameof(PackSoulGemsValue));
            }
        }
    }
}
