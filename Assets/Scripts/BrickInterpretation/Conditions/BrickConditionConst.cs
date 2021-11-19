using System;
using Newtonsoft.Json.Linq;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Conditions
{
    public sealed class BrickConditionConst : BrickCondition
    {
        public static BrickCondition Create(string typeName)
        {
            return new BrickConditionConst(typeName);
        }

        private BrickConditionConst(string typeName)
        {
            TypeName = typeName;
        }
        
        public override bool Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject valueObject 
                && valueObject.TryGetValue("value", out bool value))
            {
                return value;
            }

            throw new ArgumentException($"BrickConditionConst Run has exception! Parameters {parameters}");
        }

        public override void Reset() { }
    }
}