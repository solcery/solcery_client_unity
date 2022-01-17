using Leopotam.EcsLite;
using Solcery.BrickInterpretation.Runtime.Contexts;
using Solcery.BrickInterpretation.Runtime.Contexts.Args;
using Solcery.BrickInterpretation.Runtime.Contexts.Attrs;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.BrickInterpretation.Runtime.Contexts.Utils;
using Solcery.BrickInterpretation.Runtime.Contexts.Vars;

namespace Solcery.Games.Contexts
{
    internal class CurrentContext : IContext
    {
        public IContextObject Object { get; }
        public IContextObjectAttrs ObjectAttrs { get; }
        public IContextGameAttrs GameAttrs { get; }
        public IContextGameArgs GameArgs { get; }
        public IContextGameVars GameVars { get; }
        public IContextGameObjects GameObjects { get; }
        public ILog Log { get; }

        public static IContext Create(EcsWorld world)
        {
            return new CurrentContext(world);
        }

        private CurrentContext(EcsWorld world)
        {
            Object = CurrentContextObject.Create(world);
            ObjectAttrs = CurrentContextObjectAttrs.Create(world);
            GameAttrs = CurrentContextGameAttrs.Create(world);
            GameArgs = CurrentContextGameArgs.Create(world);
            GameVars = ComponentContextGameVars.Create(world);
            GameObjects = CurrentContextGameObjects.Create(world);
            Log = CurrentLog.Create();
        }
    }
}