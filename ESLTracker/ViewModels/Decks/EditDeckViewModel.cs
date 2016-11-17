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
        public Deck deck = new Deck() { Name = "New deck" };
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

    }
}
