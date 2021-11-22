using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionVoid : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionVoid(type, subType);
        }
        
        private BrickActionVoid(int type, int subType) : base(type, subType) { }
        
        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            Debug.Log("BrickActionVoid Run!");
        }
    }
}