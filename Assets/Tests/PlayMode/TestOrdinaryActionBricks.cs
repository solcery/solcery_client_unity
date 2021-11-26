using System;
using System.IO;
using Leopotam.EcsLite;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Solcery.BrickInterpretation;
using Solcery.Models.Context;
using Solcery.Models.Entities;
using Solcery.Models.Game;
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
            TestUtils.SetVariable(_world, "i", 10);
            ExecuteActionBrick("SetVariable");
        }
        
        [Test]
        public void TestOrdinaryActionBricksSetAttributePasses()
        {
            TestUtils.SetVariable(_world, "i", 4);
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
                
                var er = _serviceBricks.ExecuteActionBrick(brick, _world);

                if (hasArgs)
                {
                    _world.GetPool<ComponentContextArgs>().Get(argsEntityId).Pop();
                }
                
                Assert.True(er, "ExecuteActionBrick {0} execute error! Brick json {1}", brickName,
                    brick);
                return;
            }

            throw new Exception($"ExecuteActionBrick {brickName} has error!");
        }
    }
}