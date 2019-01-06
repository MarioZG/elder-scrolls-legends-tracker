using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Decks.DeckImports;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.Messages;
using ESLTracker.ViewModels.Decks.DeckImports;
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
                return deckService.LimitCardCountForDeck(deck);
            }
        }

        private bool showImportPanel = false;
        public bool ShowImportPanel
        {
            get { return showImportPanel; }
            set { SetProperty<bool>(ref showImportPanel, value); }
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
        private IDateTimeProvider dateTimeProvider;
        private IFileSaver fileManager;
        private IDeckService deckService;
        private ILogger logger;
        public DeckEditImportDeckViewModel DeckEditImportDeckDataContext { get; set; }

        public DeckEditViewModel(
            ILogger logger, 
            ICardInstanceFactory cardInstanceFactory,
            ITracker tracker,
            IMessenger messenger,
            IDateTimeProvider dateTimeProvider,
            IFileSaver fileManager,
            IDeckService deckService,
            DeckEditImportDeckViewModel deckEditImportDeckDataContext
          )
        {
            this.cardInstanceFactory = cardInstanceFactory;
            this.tracker = tracker;
            this.dateTimeProvider = dateTimeProvider;
            this.fileManager = fileManager;
            this.deckService = deckService;
            this.logger = logger;
            this.DeckEditImportDeckDataContext = deckEditImportDeckDataContext;

            this.messanger = messenger;
            messanger.Register<EditDeck>(this, EditDeckStart, EditDeck.Context.StartEdit);

            CommandSave = new RelayCommand(CommandSaveExecute);
            CommandCancel = new RelayCommand(CommandCancelExecute);
            CommandImport = new RelayCommand(CommandStartImportExecute, CommandStartImportCanExecute);
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
            SaveDeck(tracker, ver, Deck.SelectedVersion.Cards);
        }

        private void CommandCancelExecute(object obj)
        {
            this.CancelEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);
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

        private bool CommandStartImportCanExecute(object arg)
        {
            return DeckEditImportDeckDataContext.SelectedDeckImporter == null;
        }

        private void CommandStartImportExecute(object obj)
        {
            logger.Debug($"CommandStartImportExecute with type{(obj as Type)?.FullName}");
            lock (new object())
            {
                DeckEditImportDeckDataContext.SetCurrentImporter(obj as Type, this, true);
            }
            ShowImportPanel = true;
          
        }

        internal void RefreshAfterImport(bool hasWarnings)
        {
            //curr version shour equal deck.selected version, attch change to reflect clink for remove in deck history
            CurrentVersion.Cards.CollectionChanged += (s, e) => { RaisePropertyChangedEvent(nameof(ChangesFromCurrentVersion)); };
            RaisePropertyChangedEvent(String.Empty);
            ShowImportPanel = hasWarnings;
        }

    }
}