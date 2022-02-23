using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Triggers;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types.OnDrop;
using Solcery.Utils;

namespace Solcery.Models.Shared.Commands.Datas.OnDrop
{
    public sealed class CommandOnDropData : CommandData
    {
        public int ObjectId => _objectId;
        public int TargetPlaceId => _targetPlaceId;
        public TriggerTargetEntityTypes TriggerTargetEntityType => _triggerTargetEntityTypes;
        
        private readonly int _objectId;
        private readonly int _targetPlaceId;
        private readonly TriggerTargetEntityTypes _triggerTargetEntityTypes;
        
        public static CommandData CreateFromParameters(int objectId, int targetPlaceId, TriggerTargetEntityTypes triggerTargetEntityTypes)
        {
            return new CommandOnDropData(objectId, targetPlaceId, triggerTargetEntityTypes);
        }
        
        public static CommandData CreateFromJson(JObject obj)
        {
            var objectId = obj.GetValue<int>("object_id");
            var targetPlaceId = obj.GetValue<int>("target_place_id");
            var triggerTargetEntityTypes = obj.TryGetEnum("trigger_target_entity_type", out TriggerTargetEntityTypes ttet) 
                ? ttet : TriggerTargetEntityTypes.None;
            return new CommandOnDropData(objectId, targetPlaceId, triggerTargetEntityTypes);
        }
        
        private CommandOnDropData() { }
        
        private CommandOnDropData(int objectId, int targetPlaceId, TriggerTargetEntityTypes triggerTargetEntityTypes)
        {
            _objectId = objectId;
            _targetPlaceId = targetPlaceId;
            _triggerTargetEntityTypes = triggerTargetEntityTypes;
        }
        
        protected override CommandTypes GetCommandType()
        {
            return CommandTypes.OnDrop;
        }

        protected override void ConvertCommandToJson(JObject obj)
        {
            obj.Add("object_id", new JValue(_objectId));
            obj.Add("target_place_id", new JValue(_targetPlaceId));
            obj.Add("trigger_target_entity_type", new JValue((int)_triggerTargetEntityTypes));
        }

        public override void ApplyCommandToWorld(EcsWorld world)
        {
            var entityId = world.NewEntity();
            
            if (TryAddTriggerEntityType(entityId, _triggerTargetEntityTypes, world))
            {
                world.DelEntity(entityId);
                return;
            }
            
            world.GetPool<ComponentTriggerTag>().Add(entityId);
            world.GetPool<ComponentTriggerOnDropTag>().Add(entityId);
            world.GetPool<ComponentTriggerTargetObjectId>().Add(entityId).TargetObjectId = _objectId;
            world.GetPool<ComponentTriggerTargetPlaceId>().Add(entityId).TargetPlaceId = _targetPlaceId;
        }
        
        private bool TryAddTriggerEntityType(int entityId, TriggerTargetEntityTypes triggerTargetEntityType, EcsWorld world)
        {
            switch (triggerTargetEntityType)
            {
                case TriggerTargetEntityTypes.Card:
                    world.GetPool<ComponentTriggerEntityCardTag>().Add(entityId);
                    return true;
            }
            
            return false;
        }
    }
}