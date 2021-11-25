using System.IO;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Models.Context;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Tests.PlayMode
{
    public class TestCustomBrick
    {
        private IServiceBricks _serviceBricks;
        
        [SetUp]
        public void Setup()
        {
            _serviceBricks = ServiceBricks.Create();
            Assert.True(_serviceBricks != null);
            RegistrationOrdinaryBricks();
            RegistrationCustomBricks();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceBricks?.Destroy();
        }
        
        [Test]
        public void TestOrdinaryBrickSimplePasses()
        {
            var world = new EcsWorld();
            Assert.True(world != null);
            var brickJson = LoadTestBrick(Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/ordinary_brick.json"));
            Assert.True(brickJson != null);
            Assert.True(_serviceBricks.ExecuteActionBrick(brickJson, world));
        }
        
        [Test]
        public void TestCustomBrickSimplePasses()
        {
            var world = new EcsWorld();
            Assert.True(world != null);
            var entityId = world.NewEntity();
            world.GetPool<ComponentContextArgs>().Add(entityId);
            var brickJson = LoadTestBrick(Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/custom_brick.json"));
            Assert.True(brickJson != null);
            Assert.True(_serviceBricks.ExecuteValueBrick(brickJson, world, out var result));
            Assert.True(result == 10);
        }

        private void RegistrationOrdinaryBricks()
        {
            // Value bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Constant, BrickValueConst.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Variable, BrickValueVariable.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Attribute, BrickValueAttribute.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Argument, BrickValueArgument.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.IfThenElse, BrickValueIfThenElse.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Addition, BrickValueAdd.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Subtraction, BrickValueSub.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Division, BrickValueDiv.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Modulo, BrickValueMod.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Random, BrickValueRandom.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Multiplication, BrickValueMul.Create);
            
            // Action bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Void, BrickActionVoid.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.TwoActions, BrickActionTwoActions.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Argument, BrickActionArgument.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Loop, BrickActionForLoop.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetVariable, BrickActionSetVariable.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.UseCard, BrickActionUseCard.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.ConsoleLog, BrickActionConsoleLog.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.IfThenElse, BrickActionIfThenElse.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetAttribute, BrickActionSetAttribute.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.SetGameAttribute, BrickActionSetGameAttribute.Create);

            // Condition bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Constant, BrickConditionConst.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Not, BrickConditionNot.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.And, BrickConditionAnd.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Or, BrickConditionOr.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Equal, BrickConditionEqual.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.GreaterThan, BrickConditionGreaterThan.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.LesserThan, BrickConditionLesserThan.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Argument, BrickConditionArgument.Create);
        }

        private void RegistrationCustomBricks()
        {
            var customBricks = LoadTestBrick(Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/custom_bricks.json"));

            if (customBricks.TryGetValue("customBricks", out JObject customBricksObject) 
                && customBricksObject.TryGetValue("objects", out JArray bricksArray))
            {
                _serviceBricks.RegistrationCustomBricksData(bricksArray);
            }
        }

        private JObject LoadTestBrick(string path)
        {
            return JObject.Parse(File.ReadAllText(path));
        }
    }
}
