using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.DataModel;

namespace ESLTracker.ViewModels.Decks
{
    class CreateNewDeckCommand : ICommand
    {
        public void Execute(object parameter)
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
                model.Deck = new Deck();
                model.mainWindowViewModel.DeckEditVisible = false;
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
