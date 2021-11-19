using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.Models;
using Solcery.Services.Resources;
using Solcery.Services.Transport;
using Solcery.Widgets.Factory;

namespace Solcery.Games
{
    public interface IGame
    {
        public ITransportService TransportService { get; }
        public IServiceBricks ServiceBricks { get; }
        public IServiceResource ServiceResource { get; }
        public IModel Model { get; }
        public IWidgetFactory WidgetFactory { get; }
        public JObject GameContent { get; }
        public JObject GameStatePopAndClear { get; }

        public void Init();
        public void Update(float dt);
        public void Destroy();
    }
}