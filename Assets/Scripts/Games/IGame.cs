using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Games.Contents;
using Solcery.Models.Play;
using Solcery.Services.Resources;
using Solcery.Services.Transport;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets_new.Eclipse.Cards;
using Solcery.Widgets_new.Eclipse.Tokens;
using Solcery.Widgets_new.Factories;
using Solcery.Widgets_new.Tooltip;

namespace Solcery.Games
{
    public interface IGame
    {
        public ITransportService TransportService { get; }
        public IServiceBricks ServiceBricks { get; }
        public IServiceResource ServiceResource { get; }
        public IPlayModel PlayModel { get; }
        public IWidgetCanvas WidgetCanvas { get; }
        public IPlaceWidgetFactory PlaceWidgetFactory { get; }
        public IWidgetPool<ICardInContainerWidget> CardInContainerWidgetPool { get; }
        public IWidgetPool<ITokenInContainerWidget> TokenInContainerWidgetPool { get; }
        public IWidgetPool<IEclipseCardInContainerWidget> EclipseCardInContainerWidgetPool { get; }
        public JObject GameContent { get; }
        public JObject GameStatePopAndClear { get; }
        public TooltipController TooltipController { get; }
        public IGameContentAttributes GameContentAttributes { get; }

        public void Init();
        public void Update(float dt);
        public void Destroy();
    }
}