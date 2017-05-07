using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.Utils.Extensions;
using ESLTracker.Services;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckEditViewModel : ViewModelBase, IEditableObject
    {
        private Deck deck;

        public Deck Deck
        {
            get { return deck; }
            set
            {
                deck = value;
                CurrentVersion = value?.History.Where(dh => dh.VersionId == Deck.SelectedVersionId).First();
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        public ICommand CommandSave
        {
            get
            {
                return new RelayCommand(CommandSaveExecute);
            }
        }

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

        public bool AllowVersionSave
        {
            get
            {
                return (!this.trackerFactory.GetTracker().Decks.Contains(deck)) //do not allow versions for new deck
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

        private ITrackerFactory trackerFactory;
        private IMessenger messanger;

        public DeckEditViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public DeckEditViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            this.messanger = trackerFactory.GetService<IMessenger>();
            messanger.Register<EditDeck>(this, EditDeckStart, EditDeck.Context.StartEdit);
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
            SaveDeck(this.trackerFactory.GetTracker(), ver, Deck.SelectedVersion.Cards);
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

                Deck.CreateVersion(
                    major,
                    minor,
                    trackerFactory.GetDateTimeNow());

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
            trackerFactory.GetFileManager().SaveDatabase();
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
                                trackerFactory.GetService<IDeckService>().EnforceCardLimit(instance);
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
                    result.Add(new CardInstance(card.Card) { Quantity = -card.Quantity });
                }
            }

            return new ObservableCollection<CardInstance>(result.Where(ci => ci.Quantity != 0));
        }

    }
}