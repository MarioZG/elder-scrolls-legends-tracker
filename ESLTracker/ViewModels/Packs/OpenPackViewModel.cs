using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Packs
{
    public class OpenPackViewModel : ViewModelBase
    {
        private TrackerFactory trackerFactory;

        private Pack pack;

        public Pack Pack
        {
            get { return pack; }
            set { pack = value; RaisePropertyChangedEvent(nameof(Pack)); }
        }

        public IEnumerable<string> CardNamesList
        {
            get
            {
                return trackerFactory.GetCardsDatabase().CardsNames;
            }
        }

        public ICommand CommandSave
        {
            get { return new RelayCommand(new Action<object>(CommandSaveExecute)); }
        }

        public OpenPackViewModel() : this(new TrackerFactory())
        {
            
        }

        public OpenPackViewModel(TrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            InitNewPack();

        }

        private void InitNewPack()
        {
            Pack = new Pack(new List<CardInstance>()
            { new CardInstance(), new CardInstance(), new CardInstance(),
               new CardInstance(), new CardInstance(), new CardInstance()});
          

        }

        private void CommandSaveExecute(object obj)
        {
            if (pack.Cards.Any(c=> c.Card == null || c.Card == Card.Unknown))
            {
                return;
            }

            if (trackerFactory.GetSettings().Packs_ScreenshotAfterAdded)
            {
                TakePackScreenshot();
            }
            ITracker tracker = trackerFactory.GetTracker();
            Pack.DateOpened = trackerFactory.GetDateTimeNow();
            tracker.Packs.Add(Pack);
            new FileManager(trackerFactory).SaveDatabase();
            InitNewPack();
        }

        public void TakePackScreenshot()
        {
            string fileName = new ScreenshotNameProvider().GetScreenShotName(ScreenshotNameProvider.ScreenShotType.Pack);
            new FileManager(trackerFactory).SaveScreenShot(fileName);
        }
    }
}
