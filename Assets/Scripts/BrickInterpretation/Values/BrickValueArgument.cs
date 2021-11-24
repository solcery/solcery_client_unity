using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
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

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject
                && valueObject.TryGetValue("value", out string argName))
            {
                ref var contextArgs = ref world.GetPool<ComponentContextArgs>().GetRawDenseItems()[0];
                var args = contextArgs.Pop();
                if (args.TryGetValue(argName, out var brickToken) && brickToken is JObject brickObject)
                {
                    if (serviceBricks.ExecuteValueBrick(brickObject, world, out var result))
                    {
                        contextArgs.Push(args);
                        return result;
                    }
                }
            }

            throw new ArgumentException($"BrickValueArgument Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}