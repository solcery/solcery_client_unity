using System;
using System.IO;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Solcery.BrickInterpretation;
using Solcery.Models.Play.Game;
using Solcery.Models.Shared.Context;
using Solcery.Models.Shared.Entities;
using Solcery.Models.Shared.Game.Attributes;
using UnityEngine;
using Solcery.Utils;

namespace Solcery.Tests.PlayMode
{
    public class TestOrdinaryActionBricks
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
            
            _ordinaryBricks = TestUtils.LoadTestBrick(Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/test_ordinary_action_bricks.json"));
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
        public void TestOrdinaryActionBricksVoidPasses()
        {
            ExecuteActionBrick("Void");
        }
        
        [Test]
        public void TestOrdinaryActionBricksTwoActionsPasses()
        {
            ExecuteActionBrick("TwoActions");
        }
        
        [Test]
        public void TestOrdinaryActionBricksIfThenElsePasses()
        {
            ExecuteActionBrick("IfThenElse");
        }
        
        [Test]
        public void TestOrdinaryActionBricksLoopPasses()
        {
            ExecuteActionBrick("Loop");
        }
        
        [Test]
        public void TestOrdinaryActionBricksArgumentPasses()
        {
            ExecuteActionBrick("Argument");
        }
        
        [Test]
        public void TestOrdinaryActionBricksSetVariablePasses()
        {
            ExecuteActionBrick("SetVariable");
        }
        
        [Test]
        public void TestOrdinaryActionBricksSetAttributePasses()
        {
            var entityId = _world.NewEntity();
            ref var attrs = ref _world.GetPool<ComponentEntityAttributes>().Add(entityId);
            attrs.Attributes.Add("hp", 0);
            TestUtils.PushObject(_world, entityId);
            ExecuteActionBrick("SetAttribute");
            TestUtils.TryPopObject<object>(_world, out _);
        }
        
        [Test]
        public void TestOrdinaryActionBricksUseCardPasses()
        {
            ExecuteActionBrick("UseCard");
        }
        
        [Test]
        public void TestOrdinaryActionBricksSetGameAttributePasses()
        {
            var entityId = _world.NewEntity();
            ref var attrs = ref _world.GetPool<ComponentGameAttributes>().Add(entityId);
            attrs.Attributes.Add("finished", 0);
            TestUtils.PushObject(_world, entityId);
            ExecuteActionBrick("SetGameAttribute");
            TestUtils.TryPopObject<object>(_world, out _);
        }
        
        [Test]
        public void TestOrdinaryActionBricksConsoleLogPasses()
        {
            ExecuteActionBrick("ConsoleLog");
        }
        
        [Test]
        public void TestOrdinaryActionBricksIteratorPasses()
        {
            var entityId1 = _world.NewEntity();
            ref var cardId1 = ref _world.GetPool<ComponentEntityId>().Add(entityId1);
            cardId1.Id = 1;
            var entityId2 = _world.NewEntity();
            ref var cardId2 = ref _world.GetPool<ComponentEntityId>().Add(entityId2);
            cardId2.Id = 2;
            var entityId3 = _world.NewEntity();
            ref var cardId3 = ref _world.GetPool<ComponentEntityId>().Add(entityId3);
            cardId3.Id = 3;
            TestUtils.PushObject(_world, entityId3);
            ExecuteActionBrick("Iterator");
            TestUtils.TryPopObject<object>(_world, out _);
        }
        
        #endregion
        
        private void ExecuteActionBrick(string brickName)
        {
            if (_ordinaryBricks.TryGetValue(brickName, out JObject brickObject)
                && brickObject.TryGetValue("brick", out JObject brick))
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
                    ref var attrs = ref _world.GetPool<ComponentEntityAttributes>().Add(attrsEntityId);
                    foreach (var attributeToken in attributesArray)
                    {
                        if (attributeToken is JObject attributeObject 
                            && attributeObject.TryParseBrickParameter(out var key, out int value))
                        {
                            attrs.Attributes.Add(key, value);
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
                
                var er = _serviceBricks.ExecuteActionBrick(brick, _world);

                if (hasArgs)
                {
                    _world.GetPool<ComponentContextArgs>().Get(argsEntityId).Pop();
                }
                
                if (hasAttrs)
                {
                    _world.GetPool<ComponentContextObject>().Get(objectEntityId).TryPop<int>(out _);
                    _world.DelEntity(attrsEntityId);
                }
                
                Assert.True(er, "ExecuteActionBrick {0} execute error! Brick json {1}", brickName,
                    brick);
                return;
            }

            throw new Exception($"ExecuteActionBrick {brickName} has error!");
        }
    }
}