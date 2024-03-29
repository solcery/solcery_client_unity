using System;
using System.IO;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Runtime;
using Solcery.BrickInterpretation.Runtime.Utils;
using Solcery.Models.Shared.Attributes.Values;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Objects;
using UnityEngine;
using Solcery.Utils;

namespace Solcery.Tests.PlayMode
{
    public class TestOrdinaryConditionBricks
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
            
            _ordinaryBricks = TestUtils.LoadTestBrick(Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/test_ordinary_condition_bricks.json"));
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
        public void TestOrdinaryConditionBricksConstantPasses()
        {
            ExecuteConditionBrick("Constant");
        }
        
        [Test]
        public void TestOrdinaryConditionBricksNotPasses()
        {
            ExecuteConditionBrick("Not");
        }
        
        [Test]
        public void TestOrdinaryConditionBricksEqualPasses()
        {
            ExecuteConditionBrick("Equal");
        }
        
        [Test]
        public void TestOrdinaryConditionBricksGreaterThanPasses()
        {
            ExecuteConditionBrick("GreaterThan");
        }
        
        [Test]
        public void TestOrdinaryConditionBricksLesserThanPasses()
        {
            ExecuteConditionBrick("LesserThan");
        }
        
        [Test]
        public void TestOrdinaryConditionBricksArgumentPasses()
        {
            ExecuteConditionBrick("Argument");
        }
        
        [Test]
        public void TestOrdinaryConditionBricksOrPasses()
        {
            ExecuteConditionBrick("Or");
        }
        
        [Test]
        public void TestOrdinaryConditionBricksAndPasses()
        {
            ExecuteConditionBrick("And");
        }
        
        #endregion
        
        private void ExecuteConditionBrick(string brickName)
        {
            if (_ordinaryBricks.TryGetValue(brickName, out JObject brickObject)
                && brickObject.TryGetValue("brick", out JObject brick)
                && brickObject.TryGetValue("result", out bool result))
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
                
                if (brickObject.TryGetValue("variables", out JArray variableArray))
                {
                    var filter = _world.Filter<ComponentContextVars>().End();
                    foreach (var entityId in filter)
                    {
                        ref var vars = ref _world.GetPool<ComponentContextVars>().Get(entityId);
                        foreach (var variableToken in variableArray)
                        {
                            if (variableToken is JObject variableObject
                                && variableObject.TryParseBrickParameter(out var key, out int value))
                            {
                                vars.Set(key, value);
                            }
                        }
                        break;
                    }
                }
                
                var hasAttrs = false;
                var attrsEntityId = -1;
                var objectEntityId = -1;
                if (brickObject.TryGetValue("attributes", out JArray attributesArray))
                {
                    attrsEntityId = _world.NewEntity();
                    ref var attrs = ref _world.GetPool<ComponentObjectAttributes>().Add(attrsEntityId);
                    foreach (var attributeToken in attributesArray)
                    {
                        if (attributeToken is JObject attributeObject 
                            && attributeObject.TryParseBrickParameter(out var key, out int value))
                        {
                            attrs.Attributes.Add(key, AttributeValue.Create(value));
                        }
                    }
                    
                    var filter = _world.Filter<ComponentContextObject>().End();
                    foreach (var entityId in filter)
                    {
                        objectEntityId = entityId;
                        break;
                    }
                    
                    _world.GetPool<ComponentContextObject>().Get(objectEntityId).Push(attrsEntityId);
                    
                    hasAttrs = true;
                }
                
                var er = _serviceBricks.ExecuteConditionBrick(brick, Contexts.TestContext.Create(_world), 1, out var ebr);

                if (hasArgs)
                {
                    _world.GetPool<ComponentContextArgs>().Get(argsEntityId).Pop();
                }
                
                if (hasAttrs)
                {
                    _world.GetPool<ComponentContextObject>().Get(objectEntityId).TryPop<int>(out _);
                    _world.DelEntity(attrsEntityId);
                }
                
                Assert.True(er, "ExecuteConditionBrick {0} execute error! Brick json {1}", brickName, brick);
                Assert.True(result == ebr,
                    "ExecuteConditionBrick {0} execute result error! Expected Result {1}, but execute result {2}",
                    brickName, result, ebr);
                Assert.Pass("Brick name \"{0}\" Expected Result \"{1}\" execute result \"{2}\"", brickName, result, ebr);
                return;
            }

            throw new Exception($"ExecuteConditionBrick {brickName} has error!");
        }
    }
}
