using System.Collections.Generic;
using System.IO;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Models.Context;
using Solcery.Utils;

namespace Solcery.Tests.PlayMode
{
    public static class TestUtils
    {
        public  static JObject LoadTestBrick(string path)
        {
            return JObject.Parse(File.ReadAllText(path));
        }
        
        public static void RegistrationOrdinaryBricks(IServiceBricks serviceBricks)
        {
            // Value bricks
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Constant, BrickValueConst.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Variable, BrickValueVariable.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Attribute, BrickValueAttribute.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Argument, BrickValueArgument.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.IfThenElse, BrickValueIfThenElse.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Addition, BrickValueAdd.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Subtraction, BrickValueSub.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Division, BrickValueDiv.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Modulo, BrickValueMod.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Random, BrickValueRandom.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Multiplication, BrickValueMul.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.GameAttribute, BrickValueGameAttribute.Create);
            
            // Action bricks
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Void, BrickActionVoid.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.TwoActions, BrickActionTwoActions.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Argument, BrickActionArgument.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Loop, BrickActionForLoop.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetVariable, BrickActionSetVariable.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.UseCard, BrickActionUseCard.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.ConsoleLog, BrickActionConsoleLog.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.IfThenElse, BrickActionIfThenElse.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetAttribute, BrickActionSetAttribute.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetGameAttribute, BrickActionSetGameAttribute.Create);

            // Condition bricks
            serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Constant, BrickConditionConst.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Not, BrickConditionNot.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.And, BrickConditionAnd.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Or, BrickConditionOr.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Equal, BrickConditionEqual.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.GreaterThan, BrickConditionGreaterThan.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.LesserThan, BrickConditionLesserThan.Create);
            serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Argument, BrickConditionArgument.Create);
        }
        
        public static void PrepareEcsWorld(EcsWorld world)
        {
            var entityId = world.NewEntity();
            
            // Prepare context
            world.GetPool<ComponentContextObject>().Add(entityId);
            world.GetPool<ComponentContextArgs>().Add(entityId);
            world.GetPool<ComponentContextVars>().Add(entityId);
        }

        public static Dictionary<string, JObject> ParseBrickArguments(JArray argumentArray)
        {
            var argsTemp = new Dictionary<string, JObject>();
            foreach (var argumentToken in argumentArray)
            {
                if (argumentToken is JObject argumentObject 
                    && argumentObject.TryParseBrickParameter(out var name, out JObject brick))
                {
                    argsTemp.Add(name, brick);
                }
            }

            return argsTemp;
        }

        public static void PushObject(EcsWorld world, object @object)
        {
            var filter = world.Filter<ComponentContextObject>().End();
            foreach (var entityId in filter)
            {
                world.GetPool<ComponentContextObject>().Get(entityId).Push(@object);
                break;
            }
        }

        public static bool TryPopObject<T>(EcsWorld world, out T obj)
        {
            var filter = world.Filter<ComponentContextObject>().End();
            foreach (var entityId in filter)
            {
                return world.GetPool<ComponentContextObject>().Get(entityId).TryPop<T>(out obj);
            }

            obj = default;
            return false;
        }
    }
}