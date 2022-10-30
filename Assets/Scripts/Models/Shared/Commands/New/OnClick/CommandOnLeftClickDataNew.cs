using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Models.Shared.Commands.New.OnClick
{
    public sealed class CommandOnLeftClickDataNew : CommandDataNew
    {
        private readonly int _playerIndex;
        private readonly int _objectId;

        public static CommandDataNew Create(int commandId, int playerIndex, int objectId)
        {
            return new CommandOnLeftClickDataNew(commandId, CommandTypesNew.OnLeftClick, playerIndex, objectId);
        }

        public static CommandDataNew CreateFromJson(JObject commandData)
        {
            var commandId = commandData.GetValue<int>("command_id");
            var commandCtxData = commandData.GetValue<JObject>("ctx");
            var playerIndex = commandCtxData.GetValue<int>("player_index");
            var objectId = commandCtxData.GetValue<int>("object_id");
            return new CommandOnLeftClickDataNew(commandId, CommandTypesNew.OnLeftClick, playerIndex, objectId);
        }

        private CommandOnLeftClickDataNew(int commandId, CommandTypesNew commandType, int playerIndex, int objectId) : base(commandId, commandType)
        {
            _playerIndex = playerIndex;
            _objectId = objectId;
        }

        protected override void ConvertCommandToJson(JObject obj)
        {
            var ctx = new JObject
            {
                { "player_index", _playerIndex },
                { "object_id", _objectId }
            };
            obj.Add("ctx", ctx);
        }

        public override void ApplyCommandToWorld(EcsWorld world)
        {
            var entityId = world.NewEntity();
            world.GetPool<ComponentCommandTag>().Add(entityId);
            world.GetPool<ComponentCommandId>().Add(entityId).Id = CommandId;
            world.GetPool<ComponentCommandType>().Add(entityId).Type = CommandType;
            ref var commandCtx = ref world.GetPool<ComponentCommandCtx>().Add(entityId);
            commandCtx.Ctx.Add("player_index", _playerIndex);
            commandCtx.Ctx.Add("object_id", _objectId);
        }
    }
}