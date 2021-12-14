using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.Models;
using Solcery.Models.Play;
using Solcery.Services.Resources;
using Solcery.Services.Transport;
using Solcery.Widgets_new.Factories;
using Solcery.Widgets.Canvas;
using Solcery.Widgets.Factory;

namespace Solcery.Games
{
    public interface IGame
    {
        public ITransportService TransportService { get; }
        public IServiceBricks ServiceBricks { get; }
        public IServiceResource ServiceResource { get; }
        public IPlayModel PlayModel { get; }
        public IWidgetCanvas WidgetCanvas { get; }
        public IWidgetFactory WidgetFactory { get; }
        public IPlaceWidgetFactory PlaceWidgetFactory { get; }
        public JObject GameContent { get; }
        public JObject GameStatePopAndClear { get; }

        public void Init();
        public void Update(float dt);
        public void Destroy();
    }
}