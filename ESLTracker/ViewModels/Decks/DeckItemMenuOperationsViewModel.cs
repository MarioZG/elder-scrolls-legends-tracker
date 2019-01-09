using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.BusinessLogic.Decks;
using ESLTracker.BusinessLogic.Decks.DeckExports;
using TESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.DiagnosticsWrappers;
using ESLTracker.Utils.Messages;
using ESLTracker.Utils.SystemWindowsWrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Decks
{
    public class DeckItemMenuOperationsViewModel : ViewModelBase
    {
        public ICommand CommandNewDeck
        {
            get {
                return new RelayCommand(
                            (object param) => NewDeck(param as Deck));
            }
        }

        public ICommand CommandEditDeck
        {
            get {
                return new RelayCommand(
                            (object param) => CommandEditDeckExecute(param as Deck),
                            (object param) => CommandHideDeckCanExecute(param as Deck)
                            );
            }
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

        public ICommand CommandExportToSPCode
        {
            get
            {
                return new RelayCommand(
                            (object param) => CommandExportToSpCodeExecute((Deck)param),
                            (object param) => ((param is Deck) && ((Deck)param).SelectedVersion != null));
            }
        }

        private readonly IDeckService deckService;
        private readonly IMessenger messanger;
        private readonly IFileSaver fileSaver;
        private readonly ITracker tracker;
        private readonly IProcessWrapper processWrapper;
        private readonly IEnumerable<IDeckExporter> deckExporters;

        public DeckItemMenuOperationsViewModel(
            IMessenger messanger,
            IFileSaver fileSaver,
            ITracker tracker,
            IDeckService deckService,
            IProcessWrapper processWrapper,
            IEnumerable<IDeckExporter> deckExporters
           )
        {
            this.deckService = deckService;
            this.messanger = messanger;
            this.fileSaver = fileSaver;
            this.tracker = tracker;
            this.processWrapper = processWrapper;
            this.deckExporters = deckExporters;
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

        private bool CommandHideDeckCanExecute(Deck deck)
        {
            return deck != null;
        }

        public void CommandHideDeckExecute(Deck deck)
        {
            if (deck != null)
            {
                deck.IsHidden = true;
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                messanger.Send(new EditDeck() { Deck = deck }, EditDeck.Context.Hide);  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }


        public void CommandUnHideDeckExecute(Deck deck)
        {
            if (deck != null)
            {
                deck.IsHidden = false;
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                messanger.Send(new EditDeck() { Deck = deck }, EditDeck.Context.UnHide);  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }

        public void CommandDeleteDeckExecute(Deck deck)
        {
            if (deckService.CanDelete(deck))
            {
                deckService.DeleteDeck(deck);
                messanger.Send(new EditDeck() { Deck = deck });  //send message to enfore deck list refresh
                messanger.Send(new EditDeck() { Deck = deck }, EditDeck.Context.Delete);  //send message to enfore deck list refresh
                fileSaver.SaveDatabase(tracker);
            }
        }

        public void CommandOpenUrlExecute(Deck param)
        {
            processWrapper.Start(param.DeckUrl);
        }

        //command param used for deck, cannot pass export type here as in imports :(
        public void CommandExportToTextExecute(Deck deck)
        {
            if (deckService.CanExport(deck))
            {
                deckExporters.Where(de=> de.GetType() == typeof(DeckExporterText)).Single().ExportDeck(deck);
            }
        }

        //command param used for deck, cannot pass export type here as in imports :(
        public void CommandExportToSpCodeExecute(Deck deck)
        {
            if (deckService.CanExport(deck))
            {
                deckExporters.Where(de => de.GetType() == typeof(DeckExporterSPCode)).Single().ExportDeck(deck);
            }
        }
    }
}
