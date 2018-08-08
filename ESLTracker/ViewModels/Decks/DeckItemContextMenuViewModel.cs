using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckItemContextMenuViewModel : ViewModelBase
    {

        public ICommand CommandEditDeck
        {
            get { return new RelayCommand(new Action<object>(CommandEditDeckExecute)); }
        }

        public ICommand CommandHideDeck
        {
            get
            {
                return new RelayCommand(
                          (object param) => CommandHideDeckExecute(param as Deck),
                          (object param) => deckService.CommandHideDeckCanExecute(param as Deck));
            }
        }

        public ICommand CommandUnHideDeck
        {
            get
            {
                return new RelayCommand(
                  (object param) => CommandUnHideDeckExecute(param as Deck),
                  (object param) => deckService.CommandUnHideDeckCanExecute((Deck)param));
            }
        }

        public ICommand CommandDeleteDeck
        {
            get
            {
                return new RelayCommand(
                    (object param) => CommandDeleteDeckExecute(param as Deck),
                    (object param) => deckService.CanDelete(param as Deck));
            }
        }

        public ICommand CommandOpenUrl
        {
            get
            {
                return new RelayCommand(
                    (object param) => CommandOpenUrlExecute((Deck)param),
                    (object param) => ((param is Deck) && ((Deck)param).IsWebDeck));
            }
        }

        private readonly IDeckService deckService;
        private readonly IMessenger messanger;
        private readonly IFileSaver fileSaver;
        private readonly ITracker tracker;

        public DeckItemContextMenuViewModel(
            IMessenger messanger,
            IFileSaver fileSaver,
            ITracker tracker,
            IDeckService deckService)
        {
            this.deckService = deckService;
            this.messanger = messanger;
            this.fileSaver = fileSaver;
            this.tracker = tracker;
        }

        private void CommandEditDeckExecute(object param)
        {
            //inform other views that we are about to edit deck
            messanger.Send(
                new EditDeck() { Deck = (Deck)param },
                EditDeck.Context.StartEdit);
        }

        private void CommandHideDeckExecute(Deck deck)
        {
            if (deck != null)
            {
                deck.IsHidden = true;
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }


        private void CommandUnHideDeckExecute(Deck deck)
        {
            if (deck != null)
            {
                deck.IsHidden = false;
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }

        private void CommandDeleteDeckExecute(Deck deck)
        {
            if (deckService.CanDelete(deck))
            {
                deckService.DeleteDeck(deck);
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }

        private void CommandOpenUrlExecute(Deck param)
        {
            Process.Start(param.DeckUrl);
        }
    }
}
