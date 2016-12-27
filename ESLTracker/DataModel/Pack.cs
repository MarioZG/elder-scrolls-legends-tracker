using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.DataModel
{
    public class Pack
    {
        public ObservableCollection<CardInstance> Cards { get; set; } = new ObservableCollection<CardInstance>()
             { new CardInstance(Utils.CardsDatabase.Default.FindCardByName("Adoring Fan")),new CardInstance(), new CardInstance(), new CardInstance(), new CardInstance(), new CardInstance()};

        public DateTime DateOpened { get; set; }

        public int DustValue
        {
            get
            {
                return
                    Cards.Where(c => c.Card.Rarity == Enums.CardRarity.Common).Count() * 5
                    + Cards.Where(c => c.Card.Rarity == Enums.CardRarity.Rare).Count() * 20
                    + Cards.Where(c => c.Card.Rarity == Enums.CardRarity.Epic).Count() * 100
                    + Cards.Where(c => c.Card.Rarity == Enums.CardRarity.Legendary).Count() * 400;
            }
        }
    }
}
