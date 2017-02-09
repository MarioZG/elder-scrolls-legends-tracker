using System.Windows.Media;
using ESLTracker.DataModel;

namespace ESLTracker.Services
{
    public interface ICardImageService
    {
        Brush GetCardMiniature(Card card);
        Brush GetRarityBrush(Card card);
    }
}