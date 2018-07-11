using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.Services;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckEditViewModel : ViewModelBase, IEditableObject
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private ICardInstanceFactory cardInstanceFactory;
        private Deck deck;

        public Deck Deck
        {
            get { return deck; }
            set
            {
                deck = value;
                CurrentVersion = value?.History.Where(dh => dh.VersionId == deck.SelectedVersionId).First();
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        public ICommand CommandSave { get; private set; }
        public ICommand CommandCancel { get; private set; }
        public ICommand CommandImport { get; private set; }
        public ICommand CommandStartImportWeb { get; private set; }
        public ICommand CommandImportWebCancel { get; private set; }
        public IAsyncCommand<object> CommandImportWeb { get; private set; }

        private string webDeckUrl;
        public string WebDeckUrl
        {
            get { return webDeckUrl; }
            set { SetProperty<string>(ref webDeckUrl, value); }
        }

        private string webDeckUrlImportError;
        public string WebDeckUrlImportError
        {
            get { return webDeckUrlImportError; }
            set { SetProperty<string>(ref webDeckUrlImportError, value); }
        }



        public bool AllowVersionSave
        {
            get
            {
                return (!tracker.Decks.Contains(deck)) //do not allow versions for new deck
                    || Deck.IsArenaDeck(deck.Type);
            }
        }

        public string SaveCurrentLabel
        {
            get
            {
                return string.Format("Overwrite current version ({0:mm})", deck?.SelectedVersion?.Version);
            }
        }

        public string SaveMajorLabel
        {
            get
            {
                return string.Format("Major changes ({0}.0)", GetMaxMajor()+1);
            }
        }

        public string SaveMinorLabel
        {
            get
            {
                return string.Format("Small amendmends ({0}.{1})", deck?.SelectedVersion?.Version.Major, GetMaxMinor(deck?.SelectedVersion?.Version.Major) + 1);
            }
        }

        public DeckVersion currentVersion;
        public DeckVersion CurrentVersion
        {
            get
            {
                return currentVersion;
            }
            set
            {
                currentVersion = value;
                if (value != null)
                {
                    Deck.SelectedVersionId = value.VersionId;
                    //update changes
                    CurrentVersion.Cards.CollectionChanged += (s, e) => { RaisePropertyChangedEvent(nameof(ChangesFromCurrentVersion)); };
                }
                RaisePropertyChangedEvent(nameof(CurrentVersion));
                RaisePropertyChangedEvent(nameof(SaveCurrentLabel));                
            }
        }

        public bool LimitCardCount
        {
            get
            {
                return LimitCardCountForDeck(deck);
            }
        }

        private bool showImportPanel;

        public bool ShowImportPanel
        {
            get { return showImportPanel; }
            set
            {
                showImportPanel = value;
                RaisePropertyChangedEvent(nameof(ShowImportPanel));
            }
        }

        private bool showImportFromUrlPanel = false;
        public bool ShowImportFromUrlPanel
        {
            get { return showImportFromUrlPanel; }
            set { SetProperty<bool>(ref showImportFromUrlPanel, value); }
        }

        public ObservableCollection<CardInstance> ChangesFromCurrentVersion
        {
            get
            {
                if ((savedState != null)
                    && (CurrentVersion != null))
                {
                    return CalculateDeckChanges(CurrentVersion.Cards, savedState.History.Where(dv => dv.VersionId == CurrentVersion.VersionId).First().Cards);
                }
                else
                {
                    return null;
                }
            }
        }

        public Dictionary<string, ObservableCollection<CardInstance>> ChangesHistory
        {
            get
            {
                Dictionary<string, ObservableCollection<CardInstance>> changesHist = new Dictionary<string, ObservableCollection<CardInstance>>();
                Deck surceDeck = savedState != null ? savedState : Deck;
                if (surceDeck != null)
                {
                    DeckVersion prev = null;
                    foreach (DeckVersion dv in surceDeck.History)
                    {
                        if (prev != null)
                        {
                            changesHist.Add(prev.Version.ToString("mm") + " -> " + dv.Version.ToString("mm"), CalculateDeckChanges(dv.Cards, prev.Cards));
                        }
                        prev = dv;
                    }
                    changesHist = changesHist.Reverse().ToDictionary(i => i.Key, i => i.Value);
                }
                return changesHist;
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; RaisePropertyChangedEvent(nameof(ErrorMessage)); }
        }

        //priate ariable used for IEditableObject implemenation. Keeps inital state of object
        internal Deck savedState;

        private ITracker tracker;
        private IMessenger messanger;
        private IDeckImporter deckImporter;
        private IDateTimeProvider dateTimeProvider;
        private IFileSaver fileManager;
        private IDeckService deckService;
      //  private IDeckVersionFactory deckVersionFactory;

        public DeckEditViewModel(
            ICardInstanceFactory cardInstanceFactory,
            IDeckImporter deckImporter,
            ITracker tracker,
            IMessenger messenger,
            IDateTimeProvider dateTimeProvider,
            IFileSaver fileManager,
            IDeckService deckService
          //  IDeckVersionFactory deckVersionFactory
          )
        {
            this.cardInstanceFactory = cardInstanceFactory;
            this.deckImporter = deckImporter;
            this.tracker = tracker;
            this.dateTimeProvider = dateTimeProvider;
            this.fileManager = fileManager;
            this.deckService = deckService;
           // this.deckVersionFactory = deckVersionFactory;

            this.messanger = messenger;
            messanger.Register<EditDeck>(this, EditDeckStart, EditDeck.Context.StartEdit);

            CommandSave = new RelayCommand(CommandSaveExecute);
            CommandCancel = new RelayCommand(CommandCancelExecute);
            CommandImport = new RealyAsyncCommand<object>(CommandImportExecute);
            CommandStartImportWeb = new RelayCommand(CommandStartImportWebExecute, CommandStartImportWebCanExecute);
            CommandImportWebCancel = new RelayCommand(CommandImportWebCancelExecute);
            CommandImportWeb = new RealyAsyncCommand<object>(CommandImportWebExecute, CommandImportWebCanExecute);            
        }



        internal void EditDeckStart(EditDeck obj)
        {
            this.Deck = obj.Deck;
            this.BeginEdit();
        }

        private void CommandSaveExecute(object parameter)
        {
            string versionInc = parameter as string;
            SerializableVersion ver = new SerializableVersion(0, 0);
            if (parameter != null)
            {
                ver = new SerializableVersion(new Version(versionInc));
            }
            ClearModifiedBorder();
            SaveDeck(tracker, ver, Deck.SelectedVersion.Cards);
        }

        private void CommandCancelExecute(object obj)
        {
            ClearModifiedBorder();
            this.CancelEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);
        }

        private void ClearModifiedBorder()
        {
            foreach (var ci in Deck.SelectedVersion.Cards.Where(ci => ci.BorderBrush != null))
            {
                ci.BorderBrush = null;
            }
        }

        public void BeginEdit()
        {
            savedState = Deck.Clone() as Deck;
        }

        public void EndEdit()
        {
            savedState = null;
        }

        public void CancelEdit()
        {
            Deck.Name = savedState.Name;
            Deck.Class = savedState.Class;
            Deck.Type = savedState.Type;
            Deck.DeckId = savedState.DeckId;
            Deck.Notes = savedState.Notes;
            Deck.CreatedDate = savedState.CreatedDate;
            Deck.ArenaRank = savedState.ArenaRank;
            Deck.SelectedVersionId = savedState.SelectedVersionId;
            Deck.CopyHistory(savedState.History);
            Deck.IsHidden = savedState.IsHidden;
            Deck.LastUsed = savedState.LastUsed;
            Deck.DeckTag = savedState.DeckTag;
            Deck.DeckUrl = savedState.DeckUrl;
            savedState = null;
            RaisePropertyChangedEvent(nameof(Deck));
        }

        public void SaveDeck(ITracker tracker, SerializableVersion versionIncrease, IEnumerable<CardInstance> cardsCollection)
        {
            ErrorMessage = String.Empty;
            if (Deck.Class == null)
            {
                ErrorMessage = "Please select deck class";
                return;
            }
            if (versionIncrease == new SerializableVersion(0, 0))
            {
                //overwrite
                //we were working on current version - do nothing
            }
            else
            {
                //save current cards
                List<CardInstance> cards = new List<CardInstance>(Deck.SelectedVersion.Cards);
                //undo changes in curr version
                //Deck.SelectedVersion points to curret latest
                Deck.SelectedVersion.Cards = savedState.History.Where(dv => dv.VersionId == Deck.SelectedVersionId).First().Cards;
                //create new verson wih new cards

                int major, minor;
                if (versionIncrease.Major == 1)
                {
                    major = versionIncrease.Major + GetMaxMajor();
                    minor = 0;
                }
                else if (versionIncrease.Minor == 1)
                {
                    major = Deck.SelectedVersion.Version.Major;
                    minor = versionIncrease.Minor + GetMaxMinor(major);
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(versionIncrease), "Method accepts only version increase by 0 or 1");
                }

                deckService.CreateDeckVersion(
                    Deck, 
                    major,
                    minor,
                    dateTimeProvider.DateTimeNow);

                //now Deck.SelectedVersion points to new version                
                foreach (CardInstance ci in cards)
                {
                    Deck.SelectedVersion.Cards.Add((CardInstance)ci.Clone());
                }

            }
            if (!tracker.Decks.Contains(this.Deck))
            {
                tracker.Decks.Add(this.Deck);
            }
            fileManager.SaveDatabase(tracker);
            this.EndEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);
        }

        private int GetMaxMajor()
        {
            if (Deck == null)
            {
                return 0;
            }
            return Deck.History.Select(dv => dv.Version).OrderByDescending(dv => dv).First().Major;
        }

        private int GetMaxMinor(int? major)
        {
            if (Deck == null)
            {
                return 0;
            }
            else if (major.HasValue)
            {
                return Deck.History.Where(dv => dv.Version.Major == major).Select(dv => dv.Version).OrderByDescending(dv => dv).First().Minor;
            }
            else
            {
                return 0;
            }
        }



        internal bool LimitCardCountForDeck(Deck deckToCheck)
        {
            return deckToCheck?.Type == DataModel.Enums.DeckType.Constructed;
        }


        private async Task<object> CommandImportExecute(object obj)
        {
            this.ShowImportPanel = true;

            DeckImporter deckImporter = obj as DeckImporter;
            var tcs = new TaskCompletionSource<bool>();
            deckImporter.ImportFinished(tcs);

            await tcs.Task;

            bool succeed = tcs.Task.Result;

            if (succeed)
            {
                if (deckImporter.DeltaImport)
                {
                    foreach (var importedCard in deckImporter.Cards)
                    {
                        var instance = deck.SelectedVersion.Cards.Where(ci => ci.Card.Id == importedCard.CardId).FirstOrDefault();
                        if (instance != null)
                        {
                            instance.Quantity += importedCard.Quantity;
                            if (instance.Quantity <= 0)
                            {
                                deck.SelectedVersion.Cards.Remove(instance);
                            }
                            if (LimitCardCount)
                            {
                                deckService.EnforceCardLimit(instance);
                            }
                        }
                        else if (importedCard.Quantity > 0)
                        {
                            deck.SelectedVersion.Cards.Add(importedCard);
                        }
                    }
                }
                else
                {
                    deck.SelectedVersion.Cards = new PropertiesObservableCollection<CardInstance>(deckImporter.Cards);
                    //curr version shour equal deck.selected version, attch change to reflect clink for remove in deck history
                    CurrentVersion.Cards.CollectionChanged += (s, e) => { RaisePropertyChangedEvent(nameof(ChangesFromCurrentVersion)); };
                    RaisePropertyChangedEvent(String.Empty);
                }
            }
            this.ShowImportPanel = false;
            return null;
        }

        internal ObservableCollection<CardInstance> CalculateDeckChanges(ObservableCollection<CardInstance> cards1, ObservableCollection<CardInstance> cards2)
        {
            ObservableCollection<CardInstance> result = new ObservableCollection<CardInstance>();

            result = cards1.DeepCopy<ObservableCollection<CardInstance>, CardInstance>();
            foreach (CardInstance card in cards2)
            {
                CardInstance currentCard = result.Where(ci => ci.CardId == card.CardId).FirstOrDefault();
                if (currentCard != null)
                {
                    currentCard.Quantity -= card.Quantity;
                }
                else
                {
                    result.Add(cardInstanceFactory.CreateFromCard(card.Card, -card.Quantity));
                }
            }

            return new ObservableCollection<CardInstance>(result.Where(ci => ci.Quantity != 0));
        }

        private bool CommandStartImportWebCanExecute(object arg)
        {
            return true;
        }

        private void CommandStartImportWebExecute(object arg)
        {
            logger.Debug($"CommandStartImportWebExecute");
            WebDeckUrlImportError = null;
            ShowImportFromUrlPanel = true;
            var clipboardContent = Clipboard.GetText();
            if (IsValidDeckUrl(clipboardContent))
            {
                WebDeckUrl = clipboardContent;
            }
        }

        private void CommandImportWebCancelExecute(object obj)
        {
            ShowImportFromUrlPanel = false;
        }

        private async Task<object> CommandImportWebExecute(object arg)
        {
            logger.Debug($"CommandImportWebExecute {CommandImportWeb.Execution}");
            return await ImportFromWeb();
        }

        private async Task<object> ImportFromWeb()
        {
            logger.Debug($"ImportFromWeb started. Task.IsNotCompleted={CommandImportWeb.Execution?.IsNotCompleted}");
            try
            {                
                var tcs = new TaskCompletionSource<bool>();
                deckImporter.ImportFinished(tcs);
                //logger.Debug($"ImportFromWeb awaiting {CommandImportWeb.Execution}");
                //await Task.Delay(5000) ;
                await deckImporter.ImportFromWeb(WebDeckUrl);
                logger.Debug($"ImportFromWeb done. tcs={tcs.Task.Result}; Errors={deckImporter.sbErrors.ToString()}");
                if (tcs.Task.Result)
                {
                    deck.DeckUrl = WebDeckUrl;
                    deck.SelectedVersion.Cards = new PropertiesObservableCollection<CardInstance>(deckImporter.Cards);
                    deck.Name = deckImporter.DeckName;
                    deck.Class = ClassAttributesHelper.FindSingleClassByAttribute(deckImporter.Cards.SelectMany(c => c.Card.Attributes).Distinct());
                    //curr version shour equal deck.selected version, attch change to reflect clink for remove in deck history
                    CurrentVersion.Cards.CollectionChanged += (s, e) => { RaisePropertyChangedEvent(nameof(ChangesFromCurrentVersion)); };
                    RaisePropertyChangedEvent(String.Empty);
                    ShowImportFromUrlPanel = false;
                }
                else
                {
                    WebDeckUrlImportError = deckImporter.sbErrors.ToString();
                }
            }
            catch (Exception ex)
            {
                WebDeckUrlImportError = ex.Message;
            }
            return "done";
        }

        private bool CommandImportWebCanExecute(object arg)
        {
            if (! String.IsNullOrWhiteSpace(WebDeckUrl))
            {
                return IsValidDeckUrl(WebDeckUrl);
            }
            return false;
        }

        private bool IsValidDeckUrl(string url)
        {
            url = url.Trim().ToLower();
            return url.StartsWith("https://") || url.StartsWith("http://");
        }
    }
}