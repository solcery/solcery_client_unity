using System;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionConsoleLog : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionConsoleLog(type, subType);
        }
        
        private BrickActionConsoleLog(int type, int subType) : base(type, subType) { }

        public override void Reset() { }

        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count > 0 
                && parameters[0].TryParseBrickParameter(out _, out string log))
            {
                Debug.Log(log);
                return;
            }
            
            throw new Exception($"BrickActionConsoleLog Run parameters {parameters}!");
        }        
    }
}