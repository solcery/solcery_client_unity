using System;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public class BrickActionConsoleLog : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionConsoleLog(type, subType);
        }
        
        private BrickActionConsoleLog(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count > 0 
                && parameters[0] is JObject logObject
                && logObject.TryGetValue("value", out string log))
            {
                context.Log.Print(log);
                return;
            }
            
            throw new Exception($"BrickActionConsoleLog Run parameters {parameters}!");
        }        
    }
}