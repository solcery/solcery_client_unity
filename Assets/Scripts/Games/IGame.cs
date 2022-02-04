using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Runtime;
using Solcery.Models.Play;
using Solcery.Services.Resources;
using Solcery.Services.Transport;
using Solcery.Widgets_new.Canvas;
using Solcery.Widgets_new.Cards.Pools;
using Solcery.Widgets_new.Cards.Widgets;
using Solcery.Widgets_new.Factories;

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
        public JObject GameContent { get; }
        public JObject GameStatePopAndClear { get; }

        public void Init();
        public void Update(float dt);
        public void Destroy();
    }
}