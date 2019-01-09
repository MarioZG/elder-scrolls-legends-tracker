using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ESLTracker.BusinessLogic.Games;
using TESLTracker.DataModel;
using ESLTracker.Utils;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Game
{
    public class ChangeGameDeckViewModel : ViewModelBase
    {
        private ITracker tracker;
        private readonly ChangeGameDeck changeGameDeck;
        private Deck selectedDeck;
        /// <summary>
        /// deck selected in dropdown
        /// </summary>
        public Deck SelectedDeck
        {
            get { return selectedDeck; }
            set { SetProperty<Deck>(ref selectedDeck, value); RaisePropertyChangedEvent("DeckVersionList"); }
        }

        /// <summary>
        /// version selected in dropdown
        /// </summary>
        public DeckVersion SelectedVersion { get; set; }

        /// <summary>
        /// List of possible decks taht you can move game to
        /// </summary>
        public IEnumerable<Deck> DecksList
        {
            get
            {
                return tracker.Decks.Where(d => d.IsHidden == false).OrderBy( d=> d.Name);
            }
        }

        /// <summary>
        /// list of version of deck for dropdown binding
        /// </summary>
        public IEnumerable<DeckVersion> DeckVersionList
        {
            get
            {
                var dhList = SelectedDeck?.History.OrderBy(dv => dv.Version).ToList();
                SelectedVersion = dhList?.Last();
                RaisePropertyChangedEvent("SelectedVersion");
                return dhList;
            }
        }

        private bool isDeckSelectorVisible = false;
        /// <summary>
        /// indicates if control with deck selection is visible
        /// </summary>
        public bool IsDeckSelectorVisible
        {
            get { return isDeckSelectorVisible; }
            set { SetProperty<bool>(ref isDeckSelectorVisible, value); }
        }

        /// <summary>
        /// command bound to move game button
        /// </summary>
        public ICommand MoveGameToDeck
        {
            get
            {
                return new RelayCommand(MoveGameToDeckExecute);
            }
        }

        public ChangeGameDeckViewModel(ITracker tracker, ChangeGameDeck changeGameDeck)
        {
            this.tracker = tracker;
            this.changeGameDeck = changeGameDeck;
        }

        /// <summary>
        /// change deck for game
        /// </summary>
        /// <param name="obj"></param>
        private void MoveGameToDeckExecute(object obj)
        {
            TESLTracker.DataModel.Game game = obj as TESLTracker.DataModel.Game;
            if (game != null)
            {
                changeGameDeck.MoveGameBetweenDecks(game, SelectedDeck, SelectedVersion);
                IsDeckSelectorVisible = false;
            }
            else
            {
                throw new ArgumentException("Must pass game object");
            }
        }
    }
}
