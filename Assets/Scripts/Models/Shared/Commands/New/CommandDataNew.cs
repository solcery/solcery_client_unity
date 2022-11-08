using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.Models.Shared.Commands.New
{
    public class CommandDataNew
    {
        public int CommandId => _commandId;

        private static int _id = 20;

        private readonly int _cid;
        private readonly int _commandId;

        protected readonly Dictionary<string, int> Ctx;

        public static void ResetId()
        {
            _id = 20;
        }

        public static CommandDataNew CreateFromJson(JObject cmd)
        {
            return new CommandDataNew(cmd);
        }

        protected CommandDataNew(int commandId)
        {
            _id++;
            _cid = _id;
            _commandId = commandId;
            Ctx = new Dictionary<string, int>();
        }

        private CommandDataNew(JObject cmd)
        {
            _cid = cmd.GetValue<int>("id");

            var data = cmd.GetValue<JObject>("data");
            _commandId = data.GetValue<int>("command_id");
            
            Ctx = new Dictionary<string, int>();
            var ctx = data.GetValue<JObject>("ctx");
            foreach (var ctxI in ctx)
            {
                if (ctxI.Value != null)
                {
                    Ctx.Add(ctxI.Key, ctxI.Value.GetValue<int>());
                }
            }
        }

        public void ApplyCommandToWorld(EcsWorld world)
        {
            var entityId = world.NewEntity();
            world.GetPool<ComponentCommandTag>().Add(entityId);
            world.GetPool<ComponentCommandId>().Add(entityId).Id = CommandId;
            ref var commandCtx = ref world.GetPool<ComponentCommandCtx>().Add(entityId);
            foreach (var ctxI in Ctx)
            {
                commandCtx.Ctx.Add(ctxI.Key, ctxI.Value);
            }
        }

        public JObject ToJson()
        {
            var ctx = new JObject();
            foreach (var kv in Ctx)
            {
                ctx.Add(kv.Key, new JValue(kv.Value));
            }
            
            var data = new JObject
            {
                { "command_id", new JValue(_commandId) },
                { "ctx", ctx }
            };
            
            var cmd = new JObject
            {
                { "id", new JValue(_cid) },
                { "data", data }
            };
            
            return cmd;
        }
    }
}