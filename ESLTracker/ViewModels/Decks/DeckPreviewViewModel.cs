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
            SaveDeck(this.trackerFactory.GetTracker());
            this.IsInEditMode = false;
        }

        private void CommandCancelExecute(object obj)
        {
            this.CancelEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);
            this.IsInEditMode = false;
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
           // this.Deck.Class = selectedClassModel.SelectedClass.Value;
            if (!tracker.Decks.Contains(this.Deck))
            {
                tracker.Decks.Add(this.Deck);
            }
            trackerFactory.GetFileManager().SaveDatabase();
            this.EndEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);

            //this.Deck = Deck.CreateNewDeck();
            //if (selectedClassModel != null)
            //{
            //    selectedClassModel.Reset();
            //}
        }
    }
}
