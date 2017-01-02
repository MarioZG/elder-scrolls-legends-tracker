using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
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
                RaisePropertyChangedEvent("");
                RaisePropertyChangedEvent("CanChangeType");
            }
        }

        public DeckType DeckType
        {
            get
            {
                return Deck.Type;
            }
            set
            {
                Deck.Type = value;
                SetDeckName(value);
                RaisePropertyChangedEvent("Deck");
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
            DeckClassSelectorViewModel selectedClassModel = parameter as DeckClassSelectorViewModel;
            if ((selectedClassModel != null)
                && selectedClassModel.SelectedClass.HasValue)
            {
                SaveDeck(selectedClassModel, tracker);
            }
        }

        public void SaveDeck(
            IDeckClassSelectorViewModel selectedClassModel,
            ITracker tracker)
        {
            this.Deck.Class = selectedClassModel.SelectedClass.Value;
            if (!tracker.Decks.Contains(this.Deck))
            {
                tracker.Decks.Add(this.Deck);
            }
            trackerFactory.GetFileManager().SaveDatabase();
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
            DeckClassSelectorViewModel selectedClassModel = parameter as DeckClassSelectorViewModel;

            this.CancelEdit();
            messanger.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.EditFinished);
            this.Deck = CreateDefaultDeck();
            if (selectedClassModel != null)
            {
                selectedClassModel.Reset();
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

        private void SetDeckName(DeckType newType)
        {
            switch (newType)
            {
                case DataModel.Enums.DeckType.Constructed:
                    Deck.Name = String.Empty;
                    break;
                case DataModel.Enums.DeckType.VersusArena:
                    Deck.Name = string.Format(trackerFactory.GetSettings().NewDeck_VersusArenaName, trackerFactory.GetDateTimeNow());
                    break;
                case DataModel.Enums.DeckType.SoloArena:
                    Deck.Name = string.Format(trackerFactory.GetSettings().NewDeck_SoloArenaName, trackerFactory.GetDateTimeNow());
                    break;
                default:
                    throw new NotImplementedException();
            }
            RaisePropertyChangedEvent("Deck");
        }
    }
}
