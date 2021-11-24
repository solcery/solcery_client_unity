using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionUseCard : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionUseCard(type, subType);
        }
        
        private BrickActionUseCard(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            Debug.Log("Call BrickActionUseCard!");
        }
    }
}