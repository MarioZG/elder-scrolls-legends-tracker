using ESLTracker.BusinessLogic.Packs;
using TESLTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.Utils;

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
