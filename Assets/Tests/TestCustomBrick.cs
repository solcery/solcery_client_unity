using System.IO;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using Solcery.Utils;
using UnityEngine;

namespace Solcery.Tests
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
            var brickJson = LoadTestBrick(Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/custom_brick.json"));
            Assert.True(brickJson != null);
            Assert.True(_serviceBricks.ExecuteValueBrick(brickJson, world, out var result));
            Assert.True(result == 10);
        }

        private void RegistrationOrdinaryBricks()
        {
            // Value bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Value, BrickValueTypes.Const, BrickValueConst.Create);
            // Action bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Void, BrickActionVoid.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Set, BrickActionSet.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Action, BrickActionTypes.Conditional, BrickActionConditional.Create);
            // Condition bricks
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Const, BrickConditionConst.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Not, BrickConditionNot.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.And, BrickConditionAnd.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Or, BrickConditionOr.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.Equal, BrickConditionEqual.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.GreaterThan, BrickConditionGreaterThan.Create);
            _serviceBricks.RegistrationBrickType(BrickTypes.Condition, BrickConditionTypes.LesserThan, BrickConditionLesserThan.Create);
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
