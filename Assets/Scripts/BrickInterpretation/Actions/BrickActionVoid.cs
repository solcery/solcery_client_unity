using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionVoid : BrickAction
    {
        public static BrickAction Create(string typeName)
        {
            return new BrickActionVoid(typeName);
        }

        private BrickActionVoid(string typeName)
        {
            TypeName = typeName;
        }
        
        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context)
        {
            Debug.Log("BrickActionVoid Run!");
        }
    }
}