using System.Windows.Media;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Services
{
    public interface ICardImageService
    {
        Brush GetCardMiniature(Card card);
        Brush GetRarityBrush(CardRarity? rarity);
    }
}