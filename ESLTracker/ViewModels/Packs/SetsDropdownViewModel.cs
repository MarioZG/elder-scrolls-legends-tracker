using ESLTracker.BusinessLogic.Packs;
using ESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.Packs
{
    public class SetsDropdownViewModel : ViewModelBase
    {

        private readonly CardSetsListProvider cardSetsListProvider;

        public SetsDropdownViewModel(CardSetsListProvider cardSetsListProvider)
        {
            this.cardSetsListProvider = cardSetsListProvider;
        }

        public IEnumerable<CardSet> GetCardSetList(bool addAllOptionAsFisrt)
        {
            return cardSetsListProvider.GetCardSetList(addAllOptionAsFisrt);
        }
    }
}
