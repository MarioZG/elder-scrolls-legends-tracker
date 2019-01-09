using System.Windows.Media;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;

namespace ESLTracker.BusinessLogic.Cards
{
    public interface ICardImage
    {
        Brush GetCardMiniature(Card card);
        Brush GetRarityBrush(CardRarity? rarity);
    }
}