using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Play.Game;
using Solcery.Models.Shared.Game.Attributes;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Values
{
    public class BrickValueGameAttribute : BrickValue
    {
        public static BrickValue Create(int type, int subType)
        {
            return new BrickValueGameAttribute(type, subType);
        }
        
        private BrickValueGameAttribute(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override int Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 1
                && parameters[0].TryParseBrickParameter(out _, out string attrName))
            {
                var filter = world.Filter<ComponentGameAttributes>().End();
                foreach (var entityId in filter)
                {
                    ref var gameAttributesComponent = ref world.GetPool<ComponentGameAttributes>().Get(entityId);
                    var attrs = gameAttributesComponent.Attributes;
                    if (attrs.ContainsKey(attrName))
                    {
                        return attrs[attrName];
                    }
                    
                    break;
                }
            }

            throw new Exception($"BrickValueGameAttribute Run parameters {parameters}!");
        }        
    }
}