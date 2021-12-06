using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Objects;
using Solcery.Utils;

namespace Solcery.BrickInterpretation.Actions
{
    public sealed class BrickActionIterator : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionIterator(type, subType);
        }
        
        private BrickActionIterator(int type, int subType) : base(type, subType) { }

        public override void Reset() { }
        
        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world, int level)
        {
            if (parameters.Count >= 3
                && parameters[0].TryParseBrickParameter(out _, out JObject conditionBrick)
                && parameters[1].TryParseBrickParameter(out _, out JObject actionBrick)
                && parameters[2].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, world, level + 1, out var limit))
            {
                var filterObjects = world.Filter<ComponentContextObject>().End();
                var filterEntityIds = world.Filter<ComponentObjectId>().End();
                var selectedObjects = new List<int>();
                var resultObjects = new List<int>();
                foreach (var uniqObjectEntityId in filterObjects)
                {
                    ref var contextObject = ref world.GetPool<ComponentContextObject>().Get(uniqObjectEntityId);
                    var oldEntityIdExist = contextObject.TryPop<int>(out var oldEntityId);
                    foreach (var entityId in filterEntityIds)
                    {
                        selectedObjects.Add(entityId);
                    }

                    selectedObjects.Shuffle();

                    limit = limit < selectedObjects.Count ? limit : selectedObjects.Count;
                    while (limit > 0 && !selectedObjects.IsEmpty())
                    {
                        var entityId = selectedObjects.Pop();
                        contextObject.Push(entityId);
                        if (serviceBricks.ExecuteConditionBrick(conditionBrick, world, level + 1, out var conditionResult) 
                            && conditionResult)
                        {
                            resultObjects.Add(entityId);
                            --limit;
                        }

                        contextObject.TryPop<int>(out _);
                    }

                    foreach (var entityId in resultObjects)
                    {
                        contextObject.Push(entityId);
                        serviceBricks.ExecuteActionBrick(actionBrick, world, level + 1);
                        contextObject.TryPop<int>(out _);
                    }

                    if (oldEntityIdExist)
                    {
                        contextObject.Push(oldEntityId);
                    }
                    
                    break;
                }

                return;
            }
            
            throw new Exception($"BrickActionIterator Run parameters {parameters}!");
        }
    }
}