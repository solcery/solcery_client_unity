using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionArgument : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionArgument(type, subType);
        }
        
        public BrickActionArgument(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject
                && valueObject.TryGetValue("value", out string argName))
            {
                var filter = world.Filter<ComponentContextArgs>().End();

                foreach (var entityId in filter)
                {
                    ref var contextArgs = ref world.GetPool<ComponentContextArgs>().Get(entityId);
                    var args = contextArgs.Pop();
                    if (args.TryGetValue(argName, out var brickToken) && brickToken is JObject brickObject)
                    {
                        if (serviceBricks.ExecuteActionBrick(brickObject, world))
                        {
                            contextArgs.Push(args);
                            return;
                        }
                    }
                    break;
                }
            }

            throw new ArgumentException($"BrickConditionArgument Run has exception! Parameters {parameters}");
        }
    }
}