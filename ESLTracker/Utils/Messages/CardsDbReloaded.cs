using ESLTracker.BusinessLogic.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.Utils.Messages
{
    public class CardsDbReloaded
    {
        public CardsDatabase NewCardsDatabase { get; private set; }

        public CardsDbReloaded(CardsDatabase newCardsDatabase)
        {
            NewCardsDatabase = newCardsDatabase;
        }

    }
}
