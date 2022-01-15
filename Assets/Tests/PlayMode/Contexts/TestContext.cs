using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Contexts.Args;
using Solcery.BrickInterpretation.Runtime.Contexts.Attrs;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.BrickInterpretation.Runtime.Contexts.Vars;

namespace Solcery.Tests.PlayMode.Contexts
{
    internal class TestContext : IContext
    {
        public IContextObject Object { get; }
        public IContextObjectAttrs ObjectAttrs { get; }
        public IContextGameAttrs GameAttrs { get; }
        public IContextGameArgs GameArgs { get; }
        public IContextGameVars GameVars { get; }
        public IContextGameObjects GameObjects { get; }

        public static IContext Create(EcsWorld world)
        {
            return new TestContext(world);
        }

        private TestContext(EcsWorld world)
        {
            Object = TestContextObject.Create(world);
            ObjectAttrs = TestContextObjectAttrs.Create(world);
            GameAttrs = TestContextGameAttrs.Create(world);
            GameArgs = TestContextGameArgs.Create(world);
            GameVars = TestContextGameVars.Create(world);
            GameObjects = TestContextGameObjects.Create(world);
        }
    }
}