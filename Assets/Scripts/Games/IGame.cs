using Solcery.BrickInterpretation.Runtime;
using Solcery.Games.Contents;
using Solcery.Games.States.New;
using Solcery.Models.Play;
using Solcery.Services.GameContent;
using Solcery.Services.Renderer;
using Solcery.Services.Resources;
using Solcery.Services.Sound;
using Solcery.Services.Transport;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.Nft.Card;
using Solcery.Widgets_new.Eclipse.Tokens;
using Solcery.Widgets_new.Factories;
using Solcery.Widgets_new.Tooltip;
using UnityEngine;

namespace Solcery.Games
{
    public interface IGame
    {
        int PlayerIndex { get; }
        bool IsPredictable { get;}
        Camera MainCamera { get; }
        ITransportService TransportService { get; }
        IServiceBricks ServiceBricks { get; }
        IServiceResource ServiceResource { get; }
        IPlayModel PlayModel { get; }
        IWidgetCanvas WidgetCanvas { get; }
        IPlaceWidgetFactory PlaceWidgetFactory { get; }
        IServiceGameContent ServiceGameContent { get; }
        IUpdateStateQueue UpdateStateQueue { get; }
        TooltipController TooltipController { get; }
        IGameContentAttributes GameContentAttributes { get; }
        IServiceRenderWidget ServiceRenderWidget { get; }
        IServiceSound ServiceSound { get; } 

        // Widget pools
        IWidgetPool<ICardInContainerWidget> CardInContainerWidgetPool { get; }
        IWidgetPool<ITokenInContainerWidget> TokenInContainerWidgetPool { get; }
        IWidgetPool<IListTokensInContainerWidget> ListTokensInContainerWidgetPool { get; }
        IWidgetPool<IEclipseCardInContainerWidget> EclipseCardInContainerWidgetPool { get; }
        IWidgetPool<IEclipseCardInContainerWidget> EclipseCardSmallInContainerWidgetPool { get; }
        IWidgetPool<IEclipseCardNftInContainerWidget> EclipseCardNftInContainerWidgetPool { get; }

        void Init();
        void Update(float dt);
        void Destroy();
    }
}