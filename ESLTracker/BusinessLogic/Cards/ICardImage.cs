using System.Windows.Media;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.BusinessLogic.Cards
{
    public interface ICardImage
    {
        Brush GetCardMiniature(Card card);
        Brush GetRarityBrush(CardRarity? rarity);
    }
}