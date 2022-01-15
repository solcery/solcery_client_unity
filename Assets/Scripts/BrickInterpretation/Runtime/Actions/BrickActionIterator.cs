using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Utils;

namespace Solcery.BrickInterpretation.Runtime.Actions
{
    public sealed class BrickActionIterator : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionIterator(type, subType);
        }
        
        private BrickActionIterator(int type, int subType) : base(type, subType) { }

        public override void Reset() { }
        
        public override void Run(IServiceBricks serviceBricks, JArray parameters, IContext context, int level)
        {
            if (parameters.Count >= 3
                && parameters[0].TryParseBrickParameter(out _, out JObject conditionBrick)
                && parameters[1].TryParseBrickParameter(out _, out JObject actionBrick)
                && parameters[2].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, context, level + 1, out var limit))
            {
                var selectedObjects = context.GameObjects.GetAllCardTypeObject();
                selectedObjects.Shuffle();
                var resultObjects = new List<object>();
                var oldObjectExist = context.Object.TryPop(out object oldObject);

                limit = limit < selectedObjects.Count ? limit : selectedObjects.Count;
                while (limit > 0 && !selectedObjects.IsEmpty())
                {
                    var selectedObject = selectedObjects.Pop();
                    context.Object.Push(selectedObject);
                    if (serviceBricks.ExecuteConditionBrick(conditionBrick, context, level + 1, out var conditionResult) 
                        && conditionResult)
                    {
                        resultObjects.Add(selectedObject);
                        --limit;
                    }

                    context.Object.TryPop<object>(out _);
                }

                foreach (var resultObject in resultObjects)
                {
                    context.Object.Push(resultObject);
                    serviceBricks.ExecuteActionBrick(actionBrick, context, level + 1);
                    context.Object.TryPop<object>(out _);
                }

                if (oldObjectExist)
                {
                    context.Object.Push(oldObject);
                }

                return;
            }
            
            throw new Exception($"BrickActionIterator Run parameters {parameters}!");
        }
    }
}