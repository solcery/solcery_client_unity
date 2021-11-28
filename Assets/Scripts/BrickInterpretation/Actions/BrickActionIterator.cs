using System.Collections.Generic;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.Models.Context;
using Solcery.Models.Entities;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.BrickInterpretation.Actions
{
    public class BrickActionIterator : BrickAction
    {
        public static BrickAction Create(int type, int subType)
        {
            return new BrickActionIterator(type, subType);
        }
        
        private BrickActionIterator(int type, int subType) : base(type, subType) { }

        public override void Reset() { }
        
        public override void Run(IServiceBricks serviceBricks, JArray parameters, EcsWorld world)
        {
            if (parameters.Count >= 3
                && parameters[0].TryParseBrickParameter(out _, out JObject conditionBrick)
                && parameters[1].TryParseBrickParameter(out _, out JObject actionBrick)
                && parameters[2].TryParseBrickParameter(out _, out JObject valueBrick)
                && serviceBricks.ExecuteValueBrick(valueBrick, world, out var limit))
            {
                var filterObjects = world.Filter<ComponentContextObject>().End();
                var filterEntityIds = world.Filter<ComponentEntityId>().End();
                var selectedObjects = new List<int>();
                var resultObjects = new List<int>();
                foreach (var uniqEntityId in filterObjects)
                {
                    ref var contextObject = ref world.GetPool<ComponentContextObject>().Get(uniqEntityId);
                    var oldEntityIdExist = contextObject.TryPop<int>(out var oldEntityId);
                    foreach (var entityId in filterEntityIds)
                    {
                        selectedObjects.Add(world.GetPool<ComponentEntityId>().Get(entityId).Id);
                    }

                    // todo shuffle selectedObjects

                    for (var i = 0; i < selectedObjects.Count && resultObjects.Count < limit; i++)
                    {
                        contextObject.Push(selectedObjects[i]);
                        if (serviceBricks.ExecuteConditionBrick(conditionBrick, world, out var conditionResult))
                        {
                            if (conditionResult && contextObject.TryPeek<int>(out var obj))
                            {
                                resultObjects.Add(obj);
                                limit--;
                            }
                        }

                        contextObject.TryPop<int>(out _);
                    }

                    foreach (var result in resultObjects)
                    {
                        contextObject.Push(result);
                        serviceBricks.ExecuteActionBrick(actionBrick, world);
                        contextObject.TryPop<int>(out _);

                    }

                    if (oldEntityIdExist)
                    {
                        contextObject.Push(oldEntityId);
                    }
                }

                return;
            }

            Debug.Log("Call BrickActionIterator!");
        }
    }
}