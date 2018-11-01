using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ESLTracker.ViewModels.Cards
{
    public class SelectCardViewModel : ViewModelBase
    {
        public bool HasCard
        {
            get
            {
                return cardInstance?.Card != null;
            }
        }

        public bool IsPremium
        {
            get
            {
                return cardInstance?.IsPremium ?? false;
            }
            set
            {
                cardInstance.IsPremium = value;
            }
        }

        public int? Quantity
        {
            get
            {
                return cardInstance?.Quantity;
            }
        }

        private CardInstance cardInstance;
        public CardInstance CardInstance
        {
            get { return cardInstance; }
            set { SetProperty(ref cardInstance, value, onChanged: () => CardInstanceChanged()); }
        }

        public int? CardAttack
        {
            get
            {
                return cardInstance?.Card?.Attack;
            }
        }
        public int? CardHealth
        {
            get
            {
                return cardInstance?.Card?.Health;
            }
        }
        public string CardName
        {
            get
            {
                return cardInstance?.Card?.Name;
            }
            set
            {
                CardNameTyped(value);
            }
        }
        public CardType? CardType
        {
            get
            {
                return cardInstance?.Card?.Type;
            }
        }
        public string CardText
        {
            get
            {
                return cardInstance?.Card?.Text;
            }
        }
        public int? CardCost
        {
            get
            {
                return cardInstance?.Card?.Cost;
            }
        }
        public string CardImageName
        {
            get
            {
                return cardInstance?.Card?.ImageName;
                //return $"/Resources/Cards/{cardInstance?.Card?.ImageName}.png";
            }
        }

        private static readonly Brush CardForegroud = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private static readonly Brush EmptyCardForegroud = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        private Brush rarityColor;
        public Brush RarityColor
        {
            get
            {
                if (rarityColor == null)
                {
                    rarityColor = cardImage.GetRarityBrush(CardInstance?.Card?.Rarity);
                }
                return rarityColor;
            }
        }

        internal CardInstance CardNameTyped(string newName)
        {
            if (CardInstance == null)
            {
                CardInstance = cardInstanceFactory.CreateFromCard(cardsDatabase.FindCardByName(newName));
            }
            else if (CardInstance.Card?.Name != newName)
            {
                CardInstance.Card = cardsDatabase.FindCardByName(newName);
                CardInstanceChanged();
            }
            return CardInstance;
        }

        private Brush backgroundColor;
        public Brush BackgroundColor
        {
            get
            {
                if (backgroundColor == null)
                {
                    backgroundColor = cardImage.GetCardMiniature(CardInstance?.Card);
                }
                return backgroundColor;
            }
        }

        private Brush foregroundColor;
        public Brush ForegroundColor
        {
            get
            {
                if (foregroundColor == null)
                {
                    if ((CardInstance?.Card != null) && (CardInstance?.Card != Card.Unknown))
                    {
                        foregroundColor = CardForegroud;
                    }
                    else
                    {
                        foregroundColor = EmptyCardForegroud;
                    }

                }
                return foregroundColor;
            }
        }

        private readonly ICardImage cardImage;
        private readonly ICardInstanceFactory cardInstanceFactory;
        private readonly ICardsDatabase cardsDatabase;

        public SelectCardViewModel(
            ICardImage cardImage,
            ICardInstanceFactory cardInstanceFactory,
            ICardsDatabase cardsDatabase)
        {
            this.cardImage = cardImage;
            this.cardInstanceFactory = cardInstanceFactory;
            this.cardsDatabase = cardsDatabase;
        }

        public void CardInstanceChanged()
        {

            ResetImages();

            RaisePropertyChangedEvent(String.Empty);
            if (CardInstance != null)
            {
                CardInstance.PropertyChanged += (sender, e) =>
                {
                    RaisePropertyChangedEvent(e.PropertyName);
                };
            }
        }

        private void ResetImages()
        {
            foregroundColor = null;
            backgroundColor = null;
            rarityColor = null;
        }
    }
}
