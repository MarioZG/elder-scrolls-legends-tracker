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
        public ObservableCollection<Card> Cards = new ObservableCollection<Card>();
        public DateTime DateOpened { get; set; }
    }
}
