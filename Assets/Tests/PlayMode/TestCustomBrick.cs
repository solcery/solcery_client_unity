using System.IO;
using Leopotam.EcsLite;
using NUnit.Framework;
using Solcery.BrickInterpretation;
using UnityEngine;

namespace Solcery.Tests.PlayMode
{
    public class TestCustomBrick
    {
        private IServiceBricks _serviceBricks;
        private EcsWorld _world;
        
        [SetUp]
        public void Setup()
        {
            _serviceBricks = ServiceBricks.Create();
            Assert.True(_serviceBricks != null, "Create service brick error {0}", _serviceBricks);
            TestUtils.RegistrationOrdinaryBricks(_serviceBricks);
            TestUtils.RegistrationCustomBricks(_serviceBricks, Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/custom_bricks.json"));

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
        
        [Test]
        public void TestCustomBricksSimplePasses()
        {
            var brickJson = TestUtils.LoadTestBrick(Path.GetFullPath($"{Application.dataPath}/Tests/Bricks/custom_brick.json"));
            Assert.True(brickJson != null);
            Assert.True(_serviceBricks.ExecuteValueBrick(brickJson, _world, 1, out var result));
            Assert.True(result == 20);
        }
    }
}
