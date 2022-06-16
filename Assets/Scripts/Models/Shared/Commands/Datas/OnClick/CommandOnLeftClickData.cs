using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Triggers;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Models.Shared.Triggers.Types.OnClick;
using Solcery.Utils;

namespace Solcery.Models.Shared.Commands.Datas.OnClick
{
    public sealed class CommandOnLeftClickData : CommandData
    {
        public int ObjectId => _objectId;
        public TriggerTargetEntityTypes TriggerTargetEntityType => _triggerTargetEntityTypes;
        
        private readonly int _objectId;
        private readonly TriggerTargetEntityTypes _triggerTargetEntityTypes;
        
        public static CommandData CreateFromParameters(int objectId, TriggerTargetEntityTypes triggerTargetEntityTypes)
        {
            return new CommandOnLeftClickData(objectId, triggerTargetEntityTypes);
        }
        
        public static CommandData CreateFromJson(JObject obj)
        {
            var objectId = obj.GetValue<int>("object_id");
            var triggerTargetEntityTypes = obj.TryGetEnum("trigger_target_entity_type", out TriggerTargetEntityTypes ttet) 
                ? ttet : TriggerTargetEntityTypes.None;
            return new CommandOnLeftClickData(objectId, triggerTargetEntityTypes);
        }
        
        private CommandOnLeftClickData(int objectId, TriggerTargetEntityTypes triggerTargetEntityTypes)
        {
            _objectId = objectId;
            _triggerTargetEntityTypes = triggerTargetEntityTypes;
        }

        protected override CommandTypes GetCommandType()
        {
            return CommandTypes.OnLeftClick;
        }

        protected override void ConvertCommandToJson(JObject obj)
        {
            obj.Add("object_id", new JValue(_objectId));
            obj.Add("trigger_target_entity_type", new JValue((int)_triggerTargetEntityTypes));
        }

        public override void ApplyCommandToWorld(EcsWorld world)
        {
            var entityId = world.NewEntity();
            
            if (!TryAddTriggerEntityType(entityId, _triggerTargetEntityTypes, world))
            {
                world.DelEntity(entityId);
                return;
            }
            
            world.GetPool<ComponentTriggerTag>().Add(entityId);
            world.GetPool<ComponentTriggerOnClickTag>().Add(entityId);
            world.GetPool<ComponentTriggerTargetObjectId>().Add(entityId).TargetObjectId = _objectId;
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