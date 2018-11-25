using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.DiagnosticsWrappers;
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
    public class DeckItemMenuOperationsViewModel : ViewModelBase
    {
        public ICommand CommandNewDeck
        {
            get { return new RelayCommand(new Action<object>(NewDeck)); }
        }

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

        public ICommand CommandExportToText
        {
            get
            {
                return new RelayCommand(
                    (object param) => CommandExportToTextExecute((Deck)param),
                    (object param) => ((param is Deck) && ((Deck)param).SelectedVersion != null));
            }
        }

        private readonly IDeckService deckService;
        private readonly IMessenger messanger;
        private readonly IFileSaver fileSaver;
        private readonly ITracker tracker;
        private readonly IProcessWrapper processWrapper;

        public DeckItemMenuOperationsViewModel(
            IMessenger messanger,
            IFileSaver fileSaver,
            ITracker tracker,
            IDeckService deckService,
            IProcessWrapper processWrapper)
        {
            this.deckService = deckService;
            this.messanger = messanger;
            this.fileSaver = fileSaver;
            this.tracker = tracker;
            this.processWrapper = processWrapper;
        }

        public void NewDeck(object parameter)
        {
            messanger.Send(
                new Utils.Messages.EditDeck() { Deck = deckService.CreateNewDeck("New deck") },
                Utils.Messages.EditDeck.Context.StartEdit
                );
        }

        public void CommandEditDeckExecute(object param)
        {
            //inform other views that we are about to edit deck
            messanger.Send(
                new EditDeck() { Deck = (Deck)param },
                EditDeck.Context.StartEdit);
        }

        public void CommandHideDeckExecute(Deck deck)
        {
            if (deck != null)
            {
                deck.IsHidden = true;
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }


        public void CommandUnHideDeckExecute(Deck deck)
        {
            if (deck != null)
            {
                deck.IsHidden = false;
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }

        public void CommandDeleteDeckExecute(Deck deck)
        {
            if (deckService.CanDelete(deck))
            {
                deckService.DeleteDeck(deck);
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }

        public void CommandOpenUrlExecute(Deck param)
        {
            processWrapper.Start(param.DeckUrl);
        }

        private void CommandExportToTextExecute(Deck deck)
        {
            if (deckService.CanExport(deck)) {
                var cards = deck.SelectedVersion.Cards
                    .OrderBy(c => c?.Card?.Cost)
                    .ThenBy(c => c?.Card?.Name)
                    .Select(c => $"{c.Quantity} [card]{c.Card.Name}[/card]").ToList();

                cards.Insert(0, $"### {deck.Name} ###");

                System.Windows.Clipboard.SetText(string.Join(Environment.NewLine, cards));
            }
        }
    }
}
