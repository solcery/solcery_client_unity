using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games.Contents;
using Solcery.Games.States.New;
using Solcery.Models.Play;
using Solcery.Services.GameContent;
using Solcery.Services.Renderer;
using Solcery.Services.Resources;
using Solcery.Services.Transport;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.Tokens;
using Solcery.Widgets_new.Factories;
using Solcery.Widgets_new.Tooltip;
using UnityEngine;

namespace Solcery.Games
{
    public interface IGame
    {
        public Camera MainCamera { get; }
        public ITransportService TransportService { get; }
        public IServiceBricks ServiceBricks { get; }
        public IServiceResource ServiceResource { get; }
        public IPlayModel PlayModel { get; }
        public IWidgetCanvas WidgetCanvas { get; }
        public IPlaceWidgetFactory PlaceWidgetFactory { get; }
        public IWidgetPool<ICardInContainerWidget> CardInContainerWidgetPool { get; }
        public IWidgetPool<ITokenInContainerWidget> TokenInContainerWidgetPool { get; }
        IWidgetPool<IListTokensInContainerWidget> ListTokensInContainerWidgetPool { get; }
        public IWidgetPool<IEclipseCardInContainerWidget> EclipseCardInContainerWidgetPool { get; }
        public IServiceGameContent ServiceGameContent { get; }
        public IUpdateStateQueue UpdateStateQueue { get; }
        public TooltipController TooltipController { get; }
        public IGameContentAttributes GameContentAttributes { get; }
        public IServiceRenderWidget ServiceRenderWidget { get; }

        public void Init();
        public void Update(float dt);
        public void Destroy();
    }
}