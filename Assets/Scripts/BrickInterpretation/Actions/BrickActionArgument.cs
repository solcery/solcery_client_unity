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
                ref var contextArgs = ref world.GetPool<ComponentContextArgs>().GetRawDenseItems()[0];
                var args = contextArgs.Pop();
                if (args.TryGetValue(argName, out var brickToken) && brickToken is JObject brickObject)
                {
                    if (serviceBricks.ExecuteActionBrick(brickObject, world))
                    {
                        contextArgs.Push(args);
                        return;
                    }
                }
            }

            throw new ArgumentException($"BrickConditionArgument Run has exception! Parameters {parameters}");
        }
    }
}