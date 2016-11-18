using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;

namespace ESLTracker.ViewModels.Game
{
    public class EditGameViewModel : ViewModelBase
    {
        public DataModel.Game game = new DataModel.Game();
        public DataModel.Game Game
        {
            get { return game; }
            set
            {
                game = value;
                Game.PropertyChanged += Game_PropertyChanged;
                RaisePropertyChangedEvent("Game");
            }
        }

        public bool DisplayPlayerRank
        {
            get
            {
                return this.Game.Type == DataModel.Enums.GameType.PlayRanked;
            }
        }

        public bool DisplayBonusRound
        {   
            get
            {
                return this.Game.Type == DataModel.Enums.GameType.PlayRanked;
            }
        }

        public EditGameViewModel()
        {
            Game.PropertyChanged += Game_PropertyChanged;
        }

        private void Game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                RaisePropertyChangedEvent("DisplayPlayerRank");
                RaisePropertyChangedEvent("DisplayBonusRound");

                if (this.Game.Type == DataModel.Enums.GameType.PlayRanked)
                {
                    this.Game.BonusRound = false;
                }
                else
                {
                    this.Game.BonusRound = null;
                }
            }
        }

        public void UpdateBindings()
        {
            RaisePropertyChangedEvent("Game");
        }




    }
}
