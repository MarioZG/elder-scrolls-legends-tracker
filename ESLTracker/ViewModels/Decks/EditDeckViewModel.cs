using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

namespace ESLTracker.ViewModels.Decks
{
    public class EditDeckViewModel : ViewModelBase, IEditableObject
    {
        public Deck deck = CreateDefaultDeck();

        public static Deck CreateDefaultDeck()
        {
            return new Deck() { Name = "New deck" };
        }

        public Deck Deck
        {
            get { return deck; }
            set
            {
                deck = value;
                this.DeckClassModel.SelectedClass = value.Class;
                RaisePropertyChangedEvent("Deck");
                RaisePropertyChangedEvent("CanChangeType");
            }
        }

        public bool CanChangeType
        {
            get
            {
                return Deck.GetDeckGames().Count() == 0;
            }
        }

        //command for add deck button 
        public RelayCommand CommandButtonSave
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandButtonSaveExecute),
                    new Func<object, bool>(CommandButtonSaveCanExecute)
                    );
            }
        }

        //command for add deck button 
        public RelayCommand CommandButtonCancel
        {
            get
            {
                return new RelayCommand(
                    new Action<object>(CommandButtonCancelExecute),
                    new Func<object, bool>(CommandButtonCancelCanExecute)
                    );
            }
        }

        public IDeckClassSelectorViewModel DeckClassModel { get; set; }

        IMessenger messanger;
        ITracker tracker;
        private TrackerFactory trackerFactory;

        public EditDeckViewModel() : this(new TrackerFactory())
        {
        }

        internal EditDeckViewModel(TrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            tracker = trackerFactory.GetTracker();
            messanger = trackerFactory.GetMessanger();
            messanger.Register<Utils.Messages.EditDeck>(this, EditDeck, Utils.Messages.EditDeck.Context.StartEdit);
        }

        private void EditDeck(EditDeck obj)
        {
            this.Deck = obj.Deck;
            this.BeginEdit();
            
        }

        public void CommandButtonSaveExecute(object parameter)
        {
            object[] args = parameter as object[];
            EditDeckViewModel model = args[0] as EditDeckViewModel;
            DeckClassSelectorViewModel selectedClassModel = args[1] as DeckClassSelectorViewModel;
            if ((model != null)
                && (selectedClassModel != null)
                && selectedClassModel.SelectedClass.HasValue)
            {
                SaveDeck( selectedClassModel, tracker);
            }
        }

        public void SaveDeck(
            IDeckClassSelectorViewModel selectedClassModel,
            ITracker tracker)
        {
            this.Deck.Class = selectedClassModel.SelectedClass.Value;
            if (! tracker.Decks.Contains(this.Deck))
            {
                tracker.Decks.Add(this.Deck);
            }
            new FileManager(trackerFactory).SaveDatabase();
            this.EndEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);

            this.Deck = CreateDefaultDeck();
            if (selectedClassModel != null)
            {
                selectedClassModel.Reset();
            }
        }

        public bool CommandButtonSaveCanExecute(object parameter)
        {
            return true;
        }

        public void CommandButtonCancelExecute(object parameter)
        {
            object[] args = parameter as object[];
            EditDeckViewModel model = args[0] as EditDeckViewModel;
            DeckClassSelectorViewModel selectedClassModel = args[1] as DeckClassSelectorViewModel;
            if (model != null)
            {
                this.CancelEdit();
                messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);
                model.Deck = CreateDefaultDeck();
                if (selectedClassModel != null)
                {
                    selectedClassModel.Reset();
                }
            }
        }

        public bool CommandButtonCancelCanExecute(object parameter)
        {
            return true;
        }

        Deck savedState;

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
            //cannot asign to Deck - it wiil break databinding and display modified value anyway!
            Deck.Name = savedState.Name;
            Deck.Class = savedState.Class;
            Deck.Type = savedState.Type;
            Deck.DeckId = savedState.DeckId;
            Deck.Notes = savedState.Notes;
            Deck.CreatedDate = savedState.CreatedDate;
            Deck.ArenaRank = savedState.ArenaRank;
            RaisePropertyChangedEvent("Deck");
        }
    }
}
