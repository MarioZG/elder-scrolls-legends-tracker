using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.ViewModels.Decks
{
    public class EditDeckViewModel : ViewModelBase
    {
        public Deck deck = CreateDefaultDeck();

        private static Deck CreateDefaultDeck()
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

        public MainWindowViewModel mainWindowViewModel { get; set; }

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


        public void CommandButtonSaveExecute(object parameter)
        {
            object[] args = parameter as object[];
            EditDeckViewModel model = args[0] as EditDeckViewModel;
            DeckClassSelectorViewModel selectedClassModel = args[1] as DeckClassSelectorViewModel;
            if ((model != null)
                && (selectedClassModel != null)
                && selectedClassModel.SelectedClass.HasValue)
            {
                model.Deck.Class = selectedClassModel.SelectedClass.Value;
                model.Deck.Attributes.AddRange(Utils.ClassAttributesHelper.Classes[model.Deck.Class]);
                DataModel.Tracker.Instance.Decks.Add(model.Deck);
                Utils.FileManager.SaveDatabase();
                model.Deck = CreateDefaultDeck();
                model.mainWindowViewModel.DeckEditVisible = false;
                if (selectedClassModel != null)
                {
                    selectedClassModel.Reset();
                }
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
                model.Deck = CreateDefaultDeck();
                model.mainWindowViewModel.DeckEditVisible = false;
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
