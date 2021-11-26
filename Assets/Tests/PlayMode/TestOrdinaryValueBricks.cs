using System;
using System.IO;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Solcery.BrickInterpretation;
using Solcery.Models.Context;
using UnityEngine;
using Solcery.Utils;

namespace Solcery.Tests.PlayMode
{
    public class TestOrdinaryValueBricks
    {
        private IServiceBricks _serviceBricks;
        private JObject _ordinaryBricks;
        private EcsWorld _world;
        
        [SetUp]
        public void Setup()
        {
            _serviceBricks = ServiceBricks.Create();
            Assert.True(_serviceBricks != null, "Create service brick error {0}", _serviceBricks);
            TestUtils.RegistrationOrdinaryBricks(_serviceBricks);
            
            _ordinaryBricks = TestUtils.LoadTestBrick(Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/test_ordinary_value_bricks.json"));
            Assert.True(_ordinaryBricks != null, "Ordinary bricks load error {0}!", _ordinaryBricks);

            _world = new EcsWorld();
            Assert.True(_world != null, "Create ecs world error {0}", _world);
            TestUtils.PrepareEcsWorld(_world);
        }

        [TearDown]
        public void TearDown()
        {
            _world?.Destroy();
            _serviceBricks?.Destroy();
        }
        
        #region Tests
        
        [Test]
        public void TestOrdinaryValueBricksConstantPasses()
        {
            ExecuteValueBrick("Constant");
        }
        
        [Test]
        public void TestOrdinaryValueBricksVariablePasses()
        {
            ExecuteValueBrick("Variable");
        }
        
        [Test]
        public void TestOrdinaryValueBricksAttributePasses()
        {
            ExecuteValueBrick("Attribute");
        }
        
        [Test]
        public void TestOrdinaryValueBricksArgumentPasses()
        {
            ExecuteValueBrick("Argument");
        }
        
        [Test]
        public void TestOrdinaryValueBricksIfThenElsePasses()
        {
            ExecuteValueBrick("IfThenElse");
        }
        
        [Test]
        public void TestOrdinaryValueBricksAdditionPasses()
        {
            ExecuteValueBrick("Addition");
        }
        
        [Test]
        public void TestOrdinaryValueBricksSubtractionPasses()
        {
            ExecuteValueBrick("Subtraction");
        }
        
        [Test]
        public void TestOrdinaryValueBricksMultiplicationPasses()
        {
            ExecuteValueBrick("Multiplication");
        }
        
        [Test]
        public void TestOrdinaryValueBricksDivisionPasses()
        {
            ExecuteValueBrick("Division");
        }
        
        [Test]
        public void TestOrdinaryValueBricksModuloPasses()
        {
            ExecuteValueBrick("Modulo");
        }
        
        [Test]
        public void TestOrdinaryValueBricksRandomPasses()
        {
            ExecuteValueBrick("Random");
        }
        
        #endregion
        
        private void ExecuteValueBrick(string brickName)
        {
            if (_ordinaryBricks.TryGetValue(brickName, out JObject brickObject)
                && brickObject.TryGetValue("brick", out JObject brick)
                && brickObject.TryGetValue("result", out int result))
            {
                var hasArgs = false;
                var argsEntityId = -1;
                if (brickObject.TryGetValue("arguments", out JArray argumentArray))
                {
                    var filter = _world.Filter<ComponentContextArgs>().End();
                    foreach (var entityId in filter)
                    {
                        argsEntityId = entityId;
                        break;
                    }
                    
                    var args = TestUtils.ParseBrickArguments(argumentArray);
                    _world.GetPool<ComponentContextArgs>().Get(argsEntityId).Push(args);
                    hasArgs = true;
                }
                
                var er = _serviceBricks.ExecuteValueBrick(brick, _world, out var ebr);

                if (hasArgs)
                {
                    _world.GetPool<ComponentContextArgs>().Get(argsEntityId).Pop();
                }
                
                Assert.True(er, "ExecuteConditionBrick {0} execute error! Brick json {1}", brickName, brick);
                Assert.True(result == ebr,
                    "ExecuteConditionBrick {0} execute result error! Expected Result {1}, but execute result {2}",
                    brickName, result, ebr);
                return;
            }

            throw new Exception($"ExecuteConditionBrick {brickName} has error!");
        }
    }
}