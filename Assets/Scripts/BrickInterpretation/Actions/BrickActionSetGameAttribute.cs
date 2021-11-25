using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Game;
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
            if (parameters.Count >= 1
                && parameters[0] is JObject attrNameObject
                && attrNameObject.TryGetValue("value", out string attrName)
                && serviceBricks.ExecuteValueBrick(parameters[1], world, out var v1))
            {
                var filter = world.Filter<ComponentGameAttributes>().End();
                foreach (var entityId in filter)
                {
                    ref var gameAttributesComponent = ref world.GetPool<ComponentGameAttributes>().Get(entityId);
                    var attrs = gameAttributesComponent.Attributes;
                    if (attrs.ContainsKey(attrName))
                    {
                        attrs[attrName] = v1;
                        return;
                    }
                    
                    break;
                }
            }

            throw new Exception($"BrickActionSetGameAttribute Run parameters {parameters}!");
        }
    }
}