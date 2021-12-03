using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Context;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public sealed class BrickValueArgument : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueArgument(type, subType);
        }
        
        private BrickValueArgument(int type, int subType) : base(type, subType) { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            if (parameters.Count > 0 
                && parameters[0].TryParseBrickParameter(out _, out string argName))
            {
                var filter = world.Filter<ComponentContextArgs>().End();
                foreach (var entityId in filter)
                {
                    ref var contextArgs = ref world.GetPool<ComponentContextArgs>().Get(entityId);
                    var args = contextArgs.Pop();
                    if (args.TryGetValue(argName, out var brickObject))
                    {
                        if (serviceBricks.ExecuteValueBrick(brickObject, world, level + 1, out var result))
                        {
                            contextArgs.Push(args);
                            return result;
                        }
                    }
                    break;
                }
            }

            throw new ArgumentException($"BrickValueArgument Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}