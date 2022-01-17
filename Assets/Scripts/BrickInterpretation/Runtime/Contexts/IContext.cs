using Solcery.BrickInterpretation.Runtime.Contexts.Args;
using Solcery.BrickInterpretation.Runtime.Contexts.Attrs;
using Solcery.BrickInterpretation.Runtime.Contexts.Objects;
using Solcery.BrickInterpretation.Runtime.Contexts.Utils;
using Solcery.BrickInterpretation.Runtime.Contexts.Vars;

namespace Solcery.BrickInterpretation.Runtime.Contexts
{
    public interface IContext
    {
        IContextObject Object { get; }
        IContextObjectAttrs ObjectAttrs { get; }
        IContextGameAttrs GameAttrs { get; }
        IContextGameArgs GameArgs { get; }
        IContextGameVars GameVars { get; }
        IContextGameObjects GameObjects { get; }
        ILog Log { get; }
    }
}