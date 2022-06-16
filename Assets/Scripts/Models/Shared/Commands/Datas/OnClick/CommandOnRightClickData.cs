using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Triggers.EntityTypes;
using Solcery.Utils;

namespace Solcery.Models.Shared.Commands.Datas.OnClick
{
    public class CommandOnRightClickData : CommandData
    {
        private readonly int _objectId;
        private readonly TriggerTargetEntityTypes _triggerTargetEntityTypes;
        
        public static CommandData CreateFromParameters(int objectId, TriggerTargetEntityTypes triggerTargetEntityTypes)
        {
            return new CommandOnRightClickData(objectId, triggerTargetEntityTypes);
        }
        
        public static CommandData CreateFromJson(JObject obj)
        {
            var objectId = obj.GetValue<int>("object_id");
            var triggerTargetEntityTypes = obj.TryGetEnum("trigger_target_entity_type", out TriggerTargetEntityTypes ttet) 
                ? ttet : TriggerTargetEntityTypes.None;
            return new CommandOnRightClickData(objectId, triggerTargetEntityTypes);
        }
        
        private CommandOnRightClickData(int objectId, TriggerTargetEntityTypes triggerTargetEntityTypes)
        {
            _objectId = objectId;
            _triggerTargetEntityTypes = triggerTargetEntityTypes;
        }
        
        protected override CommandTypes GetCommandType()
        {
            return CommandTypes.OnRightClick;
        }

        protected override void ConvertCommandToJson(JObject obj)
        {
        }

        public override void ApplyCommandToWorld(EcsWorld world)
        {
        }
    }
}