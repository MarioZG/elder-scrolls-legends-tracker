using System;
using System.Collections.Generic;
using Drawing = System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ESLTracker.DataModel;
using ESLTracker.Utils.Extensions;
using Media = System.Windows.Media;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;

namespace ESLTracker.BusinessLogic.Cards
{
    public class CardImage : ICardImage
    {

        internal static Dictionary<Guid, Media.Brush> CardMiniatureCache = new Dictionary<Guid, Media.Brush>();
        internal static Dictionary<CardRarity, Media.Brush> RarityCache = new Dictionary<CardRarity, Media.Brush>();

        private static readonly Media.SolidColorBrush EmptyRarityBrush = new Media.SolidColorBrush(Media.Color.FromArgb(255, 255, 255, 255));
        private static readonly Media.SolidColorBrush EmptyCardBrush = new Media.SolidColorBrush(Media.Color.FromArgb(255, 255, 255, 255));

        private readonly IResources resourceService;
        private readonly ILogger logger;

        public CardImage(ILogger logger, IResources resourceService)
        {
            this.resourceService = resourceService;
            this.logger = logger;
        }

        public Media.Brush GetCardMiniature(Card card)
        {
            logger?.Debug("Start GetCardMiniature");
            Media.Brush returnValue = null;
            if((card != null) && (card != Card.Unknown))
            {
                if (!CardMiniatureCache.ContainsKey(card.Id))
                {
                    Drawing.Bitmap attribsBitmap = GetAtrriutesBackground(card);
                    Drawing.Bitmap cardBitmap = GetCardSmallImage(card);

                    Media.Brush brush = MergeAttribsAndCArd(attribsBitmap, cardBitmap);

                    CardMiniatureCache.Add(card.Id, brush);
                }
                returnValue = CardMiniatureCache[card.Id];
            }
            else
            {
                returnValue = EmptyCardBrush;
            }
            logger?.Debug("End GetCardMiniature");
            return returnValue;
        }

        public Media.Brush GetRarityBrush(CardRarity? rarity)
        {
            logger?.Debug("Start GetRarityBrush");
            Media.Brush returnValue = null;
            if (rarity.HasValue)
            {
                if (!RarityCache.ContainsKey(rarity.Value))
                {
                    switch (rarity)
                    {
                        case CardRarity.Common:
                            returnValue = new Media.RadialGradientBrush(
                                 Media.Color.FromArgb(255, 115, 115, 115),
                                 Media.Color.FromArgb(255, 200, 200, 200))
                            { RadiusX = 0.6, RadiusY = 0.6 };
                            break;
                        case CardRarity.Rare:
                            returnValue = new Media.RadialGradientBrush(
                                 Media.Color.FromArgb(255, 72, 132, 226),
                                 Media.Color.FromArgb(255, 135, 195, 224))
                            { RadiusX = 0.6, RadiusY = 0.6 };
                            break;
                        case CardRarity.Epic:
                            returnValue = new Media.RadialGradientBrush(
                                Media.Color.FromArgb(255, 138, 43, 226),
                                Media.Color.FromArgb(255, 204, 84, 199))
                            { RadiusX = 0.6, RadiusY = 0.6 };
                            break;
                        case CardRarity.Legendary:
                            returnValue = new Media.RadialGradientBrush(
                                Media.Color.FromArgb(255, 240, 154, 35),
                                Media.Color.FromArgb(255, 255, 255, 40))
                            { RadiusX = 0.6, RadiusY = 0.6 };
                            break;
                        default:
                            throw new NotImplementedException("Unknown card rarity");
                    }
                    RarityCache.Add(rarity.Value, returnValue);
                }
                else
                {
                    returnValue = RarityCache[rarity.Value];
                }
            }
            else
            {
                returnValue = EmptyRarityBrush;
            }
            logger?.Debug("End GetRarityBrush");
            return returnValue;
        }

        private Media.Brush MergeAttribsAndCArd(Drawing.Bitmap attribsBitmap, Drawing.Bitmap cardBitmap)
        {
            var bitmap = new Drawing.Bitmap(269, 44);
            using (Drawing.Graphics g = Drawing.Graphics.FromImage(bitmap))
            {
             //   g.DrawImage(attribsBitmap, 0, 0);
                g.DrawImage(cardBitmap, 0, 0, 269, 44);
            }

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new Media.Imaging.BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = Media.Imaging.BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return new Media.ImageBrush(bitmapImage) { Stretch = Media.Stretch.Fill };
            }
        }

        private Drawing.Bitmap GetCardSmallImage(Card card)
        {
            Drawing.Bitmap cardBitmap;

            Uri imageUri = new Uri(card.ImageName, UriKind.RelativeOrAbsolute);
            if (resourceService.ResourceExists(imageUri))
            {
                cardBitmap = new Drawing.Bitmap(Application.GetResourceStream(imageUri).Stream);
            }
            else
            {
                cardBitmap = new Drawing.Bitmap(269, 44);
                Drawing.Brush brush = new Drawing.SolidBrush(Drawing.Color.FromArgb(255, 0, 0, 0));
                using (Drawing.Graphics graphics = Drawing.Graphics.FromImage(cardBitmap))
                {
                    graphics.FillRectangle(brush, new Drawing.Rectangle(0, 0, 269, 44));
                }
            }
            return cardBitmap;
        }

        private Drawing.Bitmap GetAtrriutesBackground(Card card)
        {
            Drawing.Color colorTo;
            Drawing.Color colorFrom;

            if (card.Attributes.Count == 1)
            {
                Drawing.Color baseColor = ClassAttributesHelper.DeckAttributeColors[card.Attributes[0]];
                colorTo = colorFrom = baseColor.ApplyFactor(0.7);
            }
            else
            {
                colorTo = ClassAttributesHelper.DeckAttributeColors[card.Attributes[0]];
                colorFrom = ClassAttributesHelper.DeckAttributeColors[card.Attributes[1]];

                colorTo = colorTo.ApplyFactor(0.7);
                colorFrom = colorFrom.ApplyFactor(0.7);
            }

            Drawing.Bitmap attribsBitmap = new Drawing.Bitmap(160, 44);
            Drawing.Brush brush = new LinearGradientBrush(
                new Drawing.PointF(39, (float)0),
                new Drawing.PointF(120, (float)0),
                colorFrom,
                colorTo);
            Drawing.Brush brushFrom = new Drawing.SolidBrush(colorFrom);
            Drawing.Brush brushTo = new Drawing.SolidBrush(colorTo);

            using (Drawing.Graphics graphics = Drawing.Graphics.FromImage(attribsBitmap))
            {
                graphics.FillRectangle(brushFrom, new Drawing.Rectangle(0, 0, 40, 44));
                graphics.FillRectangle(brush, new Drawing.Rectangle(40, 0, 80, 44));
                graphics.FillRectangle(brushTo, new Drawing.Rectangle(120, 0, 40, 44));
            }

            return attribsBitmap;
        }
    }
}
