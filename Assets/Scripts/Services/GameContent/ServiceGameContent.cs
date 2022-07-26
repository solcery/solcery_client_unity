using Newtonsoft.Json.Linq;
using Solcery.Services.GameContent.Items;
using Solcery.Utils;

namespace Solcery.Services.GameContent
{
    public sealed class ServiceGameContent : IServiceGameContent
    {
        IItemTypes IServiceGameContent.ItemTypes => _itemTypes;
        JArray IServiceGameContent.Places => _places;
        JArray IServiceGameContent.DragDrop => _dragDrop;
        JArray IServiceGameContent.Tooltips => _tooltips;
        
        private IItemTypes _itemTypes;
        private JArray _places;
        private JArray _dragDrop;
        private JArray _tooltips;

        public static IServiceGameContent Create()
        {
            return new ServiceGameContent();
        }

        private ServiceGameContent() { }

        void IServiceGameContent.UpdateGameContent(JObject data)
        {
            _itemTypes = ItemTypes.Create(data.GetValue<JObject>("card_types").GetValue<JArray>("objects"));
            _places = data.GetValue<JObject>("places").GetValue<JArray>("objects");
            _dragDrop = data.GetValue<JObject>("drag_n_drops").GetValue<JArray>("objects");
            _tooltips = data.GetValue<JObject>("tooltips").GetValue<JArray>("objects");
        }

        void IServiceGameContent.UpdateGameContentOverrides(JObject data)
        {
            _itemTypes?.UpdateOverridesItems(data.GetValue<JArray>("objects"));
        }

        void IServiceGameContent.Cleanup()
        {
            _itemTypes?.Cleanup();
            _itemTypes = null;
            _places = null;
            _dragDrop = null;
            _tooltips = null;
        }
    }
}