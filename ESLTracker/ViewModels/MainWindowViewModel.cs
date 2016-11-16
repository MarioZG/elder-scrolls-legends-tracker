using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool deckEditVisible;

        public bool DeckEditVisible
        {
            get { return deckEditVisible; }
            set { deckEditVisible = value; RaisePropertyChangedEvent("DeckEditVisible"); }
        }

    }
}
