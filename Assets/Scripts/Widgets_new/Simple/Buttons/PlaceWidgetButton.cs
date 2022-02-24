using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Games;
using Solcery.Models.Shared.Commands.Datas.OnClick;
using Solcery.Models.Shared.Objects;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Utils;
using Solcery.Widgets_new.Canvas;

namespace Solcery.Widgets_new.Simple.Buttons
{
    public sealed class PlaceWidgetButton : PlaceWidget<PlaceWidgetButtonLayout>
    {
        public static PlaceWidget Create(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject)
        {
            return new PlaceWidgetButton(widgetCanvas, game, prefabPathKey, placeDataObject);
        }
        
        private PlaceWidgetButton(IWidgetCanvas widgetCanvas, IGame game, string prefabPathKey, JObject placeDataObject) 
            : base(widgetCanvas, game, prefabPathKey, placeDataObject) { }

        public override void Update(EcsWorld world, int[] entityIds)
        {
            Layout.UpdateVisible(entityIds.Length > 0);
            Layout.ClearAllOnClickListener();
            
            if (entityIds.Length <= 0)
            {
                return;
            }

            var entityId = entityIds[0];
            var objectIdPool = world.GetPool<ComponentObjectId>();

            if (!objectIdPool.Has(entityId))
            {
                Layout.UpdateButtonText("No card id.");
                return;
            }

            var objectId = objectIdPool.Get(entityId).Id;
            
            Layout.AddOnClickListener(() =>
            {
                var command = CommandOnClickData.CreateFromParameters(objectId, TriggerTargetEntityTypes.Card);
                Game.TransportService.SendCommand(command.ToJson());
            });
            
            var objectTypesFilter = world.Filter<ComponentObjectTypes>().End();
            var objectTypePool = world.GetPool<ComponentObjectType>();
            foreach (var objectTypesEntityId in objectTypesFilter)
            {
                var objectTypes = world.GetPool<ComponentObjectTypes>().Get(objectTypesEntityId).Types;
                if (!objectTypePool.Has(entityId))
                {
                    break;
                }

                if (objectTypes.TryGetValue(objectTypePool.Get(entityId).Type, out var objectTypeDataObject) 
                    && objectTypeDataObject.TryGetValue("name", out string name))
                {
                    Layout.UpdateButtonText(name);
                    return;
                }
                
                break;
            }
            
            Layout.UpdateButtonText("No card type data.");
        }
    }
}