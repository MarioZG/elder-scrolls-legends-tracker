using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;

namespace ESLTracker.ViewModels.Decks
{
    public class EditDeckViewModel : ViewModelBase
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
                RaisePropertyChangedEvent("Deck");
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

        public EditDeckViewModel()
        {
            Utils.Messenger.Default.Register<Utils.Messages.EditDeck>(this, EditDeck, Utils.Messages.EditDeck.Context.StartEdit);
        }

        private void EditDeck(EditDeck obj)
        {
            this.Deck = obj.Deck;
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
                SaveDeck( selectedClassModel, Tracker.Instance);
            }
        }

        public void SaveDeck(
            IDeckClassSelectorViewModel selectedClassModel,
            ITracker tracker)
        {
            this.Deck.Class = selectedClassModel.SelectedClass.Value;
            this.Deck.Attributes.Clear();
            this.Deck.Attributes.AddRange(Utils.ClassAttributesHelper.Classes[this.Deck.Class]);
            if (! tracker.Decks.Contains(this.Deck))
            {
                tracker.Decks.Add(this.Deck);
            }
            Utils.FileManager.SaveDatabase();
            Messenger.Default.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.Saved);

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
                Messenger.Default.Send(new Utils.Messages.EditDeck() { Deck = this.Deck }, Utils.Messages.EditDeck.Context.Cancel);
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


    }
}
