using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.ViewModels;


namespace ESLTracker.DataModel
{
    [DebuggerDisplay("Card={Card.Name}; Golden={IsGolden}")]
    public class CardInstance : ViewModelBase
    {
        private ITrackerFactory trackerFactory;

        public Guid CardId
        {
            get
            {
                return Card != null ? Card.Id : Card.Unknown.Id;
            }
            set
            {
                LoadCardFromDataBase(value);
            }
        }

        Card card;
        [XmlIgnore]
        public Card Card
        {
            get { return card; }
            set { card = value; }
        }
        private bool isGolden;

        public bool IsGolden
        {
            get { return isGolden; }
            set { isGolden = value;  RaisePropertyChangedEvent(nameof(IsGolden)); }
        }

        public Brush BackgroundColor
        {
            get
            {
                if ((Card != null) && (Card != Card.Unknown))
                {
                    string uriAttribs = "pack://application:,,,/Resources/DeckAttribute/" + card.Attributes.ToString("a") + "back.png";
                    Uri attribsUri = new Uri(uriAttribs, UriKind.RelativeOrAbsolute);

                    System.Drawing.Bitmap attribsBitmap;
                    System.Drawing.Bitmap cardBitmap;

                    System.Drawing.Color colorTo;
                    System.Drawing.Color colorFrom;

                    if (card.Attributes.Count == 1)
                    {
                        System.Drawing.Color baseColor = ClassAttributesHelper.DeckAttributeColors[card.Attributes[0]];
                        colorTo = colorFrom = baseColor.ApplyFactor(0.7);
                    }
                    else
                    {
                        colorTo = ClassAttributesHelper.DeckAttributeColors[card.Attributes[0]];
                        colorFrom =  ClassAttributesHelper.DeckAttributeColors[card.Attributes[1]];

                        colorTo = colorTo.ApplyFactor(0.7);
                        colorFrom = colorFrom.ApplyFactor(0.7);
                    }

                    attribsBitmap = new System.Drawing.Bitmap(160, 44);
                    System.Drawing.Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new System.Drawing.PointF(39, (float)0),
                        new System.Drawing.PointF(120, (float)0),
                        colorFrom, 
                        colorTo);
                    System.Drawing.Brush brushFrom = new System.Drawing.SolidBrush(
                        colorFrom);
                    System.Drawing.Brush brushTo = new System.Drawing.SolidBrush(
                        colorTo);

                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(attribsBitmap))
                    {
                        graphics.FillRectangle(brushFrom, new System.Drawing.Rectangle(0, 0, 40, 44));
                        graphics.FillRectangle(brush, new System.Drawing.Rectangle(40, 0, 80, 44));
                        graphics.FillRectangle(brushTo, new System.Drawing.Rectangle(120, 0, 40, 44));
                    }


                    Uri imageUri = new Uri(ImageSource, UriKind.RelativeOrAbsolute);
                    if (ResourcesHelper.ResourceExists(imageUri))
                    {
                        cardBitmap = new System.Drawing.Bitmap(Application.GetResourceStream(imageUri).Stream);
                    }
                    else
                    {
                        cardBitmap = new System.Drawing.Bitmap(269, 44);
                        brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 0, 0, 0));
                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(cardBitmap))
                        {
                            graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, 269, 44));
                        }
                    }

                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(160 + 269, 44);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        g.DrawImage(attribsBitmap, 0, 0);
                        g.DrawImage(cardBitmap, 160, 0, 265, 44);
                    }

                    using (MemoryStream memory = new MemoryStream())
                    {
                        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        memory.Position = 0;
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        return new ImageBrush(bitmapImage) { Stretch = Stretch.Fill };
                    }
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                }
            }
        }

        public Brush ForegroundColor
        {
            get
            {
                if ((Card != null) && (Card != Card.Unknown))
                {
                    return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                }
            }
        }

        public Brush RarityColor
        {
            get
            {
                if ((Card != null) && (Card != Card.Unknown))
                {
                    switch (Card.Rarity)
                    {
                        case Enums.CardRarity.Common:
                            return new RadialGradientBrush(
                                 Color.FromArgb(255, 115, 115, 115),
                                 Color.FromArgb(255, 200, 200, 200))
                                  { RadiusX = 0.6, RadiusY = 0.6 };
                        case Enums.CardRarity.Rare:
                            return new RadialGradientBrush(
                                 Color.FromArgb(255, 72, 132, 226),
                                 Color.FromArgb(255, 135, 195, 224))
                                { RadiusX = 0.6, RadiusY = 0.6 };
                        case Enums.CardRarity.Epic:
                            return new RadialGradientBrush(
                                Color.FromArgb(255, 138, 43, 226),
                                Color.FromArgb(255, 204, 84, 199))
                                 { RadiusX = 0.6, RadiusY = 0.6 };
                        case Enums.CardRarity.Legendary:
                            return new RadialGradientBrush(                                
                                Color.FromArgb(255, 240, 154, 35),
                                Color.FromArgb(255, 255, 255, 40))
                                 { RadiusX = 0.6, RadiusY = 0.6 };
                        default:
                            throw new NotImplementedException("Unknown card rarity");
                    }                    
                }
                else
                {
                    return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                }
            }
        }

        public string ImageSource
        {
            get
            {
                Regex rgx = new Regex("[^a-zA-Z0-9]");
                if (Card != null)
                {
                    var name = rgx.Replace(Card?.Name, "");
                    return "pack://application:,,,/Resources/Cards/" + name + ".png";
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        
        public bool HasCard
        {
            get
            {
                return ((card != null) && (card != Card.Unknown));
            }
        }
        public CardInstance() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public CardInstance(Card card) : this(card, TrackerFactory.DefaultTrackerFactory)
        {

        }

        public CardInstance(ITrackerFactory trackerFactory) : this(null, trackerFactory)
        {
            
        }

        public CardInstance(Card card, ITrackerFactory trackerFactory)
        {
            this.Card = card;
            this.trackerFactory = trackerFactory;
        }

        private void LoadCardFromDataBase(Guid value)
        {
            this.Card = trackerFactory.GetCardsDatabase().FindCardById(value);
        }
    }
}
