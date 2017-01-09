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
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        private bool isInEditMode = false;
        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value; RaisePropertyChangedEvent(nameof(IsInEditMode)); }
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

        public DeckVersion CurrentVersion
        {
            get
            {
                return Deck?.History.Where(dh => dh.VersionId == Deck.SelectedVersionId).First();
            }
        }
        public string CurrentVersionString
        {
            get
            {
                return CurrentVersion?.Version?.ToString("v{M}.{m}");
            }
        }

        public int? MaxSingleCardQuantity
        {
            get
            {
                return GetMaxSingleCardForDeck(deck);
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
            ClearModifiedBorder();
            SaveDeck(this.trackerFactory.GetTracker());
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

        public void SaveDeck(ITracker tracker)
        {
            if (!tracker.Decks.Contains(this.Deck))
            {
                tracker.Decks.Add(this.Deck);
            }
            trackerFactory.GetFileManager().SaveDatabase();
            this.EndEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);
        }

        internal int? GetMaxSingleCardForDeck(Deck deckToCheck)
        {
            switch (deckToCheck?.Type)
            {
                case DataModel.Enums.DeckType.Constructed:
                    return 3;
                case DataModel.Enums.DeckType.VersusArena:
                    return null;
                case DataModel.Enums.DeckType.SoloArena:
                    return null;
                default:
                    return null;
            }
        }

    }
}
