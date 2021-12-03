using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Context;
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

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
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
                        if (serviceBricks.ExecuteActionBrick(brickObject, world, level + 1))
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