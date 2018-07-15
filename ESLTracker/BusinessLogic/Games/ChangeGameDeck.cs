using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Games
{
    public class ChangeGameDeck
    {
        public void MoveGameBetweenDecks(Game game, Deck tragetDeck, DeckVersion selectedVersion)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            if (tragetDeck == null)
            {
                throw new ArgumentNullException(nameof(tragetDeck));
            }

            game.Deck = tragetDeck;
            game.DeckVersionId = selectedVersion.VersionId;
        }
        
    }
}
