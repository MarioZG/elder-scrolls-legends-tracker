using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.Utils.Extensions;
using System.Collections.ObjectModel;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckPreviewViewModel : ViewModelBase, IEditableObject
    {

        private Deck deck;

        public Deck Deck
        {
            get { return deck; }
            set {
                deck = value;
                CurrentVersion = Deck?.History.Where(dh => dh.VersionId == Deck.SelectedVersionId).First();
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        private bool isInEditMode = false;
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value;
                RaisePropertyChangedEvent(nameof(IsInEditMode));
                RaisePropertyChangedEvent(nameof(ShowEditDeckPanel));
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
            }
        }

        public string CurrentVersionString
        {
            get
            {
                return CurrentVersion?.Version?.ToString("v{M}.{m}");
            }
        }

        public bool LimitCardCount
        {
            get
            {
                return LimitCardCountForDeck(deck);
            }
        }

        public bool ShowEditDeckPanel
        {
            get { return isInEditMode && ! showImportPanel; }
        }

        private bool showImportPanel;

        public bool ShowImportPanel
        {
            get { return showImportPanel; }
            set {
                showImportPanel = value;
                RaisePropertyChangedEvent(nameof(ShowImportPanel));
                RaisePropertyChangedEvent(nameof(ShowEditDeckPanel));
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
                            changesHist.Add(prev.Version.ToString("mm")+" -> "+ dv.Version.ToString("mm"), CalculateDeckChanges(dv.Cards, prev.Cards));
                        }
                        prev = dv;
                    }
                    changesHist = changesHist.Reverse().ToDictionary(i => i.Key, i => i.Value);
                }
                return changesHist;
            }
        }

        //priate ariable used for IEditableObject implemenation. Keeps inital state of object
        internal Deck savedState;

        private ITrackerFactory trackerFactory;
        private IMessenger messanger;

        public DeckPreviewViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public DeckPreviewViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            this.messanger = trackerFactory.GetMessanger();
            messanger.Register<EditDeck>(this, EditDeckStart, EditDeck.Context.StartEdit);
        }

        internal void EditDeckStart(EditDeck obj)
        {
            this.Deck = obj.Deck;
            this.BeginEdit();
            this.IsInEditMode = true;
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
            this.IsInEditMode = false;
        }

        private void CommandCancelExecute(object obj)
        {
            ClearModifiedBorder();
            this.CancelEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);
            this.IsInEditMode = false;
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
            savedState = Deck;
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
            RaisePropertyChangedEvent(nameof(Deck));
        }

        public void SaveDeck(ITracker tracker, SerializableVersion versionIncrease, IEnumerable<CardInstance> cardsCollection)
        {
            if (versionIncrease == new SerializableVersion(0,0))
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
                    major = versionIncrease.Major + Deck.History.Select(dv => dv.Version).OrderByDescending(dv => dv).First().Major;
                    minor = 0;
                }
                else if (versionIncrease.Minor == 1)
                {
                    major = Deck.SelectedVersion.Version.Major;
                    minor = versionIncrease.Minor + Deck.History.Where(dv=> dv.Version.Major == major).Select(dv => dv.Version).OrderByDescending(dv => dv).First().Minor;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(versionIncrease),"Method accepts only version increase by 0 or 1");
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
                    foreach (var importedCard in deckImporter.Cards) {
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
                                DeckHelper.EnforceCardLimit(instance);
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
                    deck.SelectedVersion.Cards = new ObservableCollection<CardInstance>(deckImporter.Cards);
                    RaisePropertyChangedEvent(String.Empty);
                }
            }
            this.ShowImportPanel = false;
            return null;
        }

        internal ObservableCollection<CardInstance> CalculateDeckChanges(ObservableCollection<CardInstance> cards1, ObservableCollection<CardInstance> cards2)
        {
            ObservableCollection<CardInstance> result = new ObservableCollection<CardInstance>();

            result = cards1.DeepCopy<CardInstance>();
            foreach(CardInstance card in cards2)
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

            return new ObservableCollection<CardInstance>(result.Where( ci=> ci.Quantity != 0));
        }

    }
}
