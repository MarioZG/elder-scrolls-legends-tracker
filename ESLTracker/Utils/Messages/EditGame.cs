using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.DataModel;

namespace ESLTracker.Utils.Messages
{
    public class EditGame
    {
        public enum Context
        {
            StartEdit,
            EditFinished
        }

        public Game Game { get; set; }

        public EditGame(Game game)
        {
            Game = game;
        }

    }
}
