using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.Game;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionSetGameAttribute : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionSetGameAttribute(type, subType);
        }
        
        private BrickActionSetGameAttribute(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 2
                && parameters[0].TryParseBrickParameter(out _, out string attrName)
                && parameters[1].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, world, out var value))
            {
                var filter = world.Filter<ComponentGameAttributes>().End();
                foreach (var entityId in filter)
                {
                    ref var gameAttributesComponent = ref world.GetPool<ComponentGameAttributes>().Get(entityId);
                    var attrs = gameAttributesComponent.Attributes;
                    if (attrs.ContainsKey(attrName))
                    {
                        attrs[attrName] = value;
                        return;
                    }
                    
                    break;
                }
            }

            throw new Exception($"BrickActionSetGameAttribute Run parameters {parameters}!");
        }
    }
}