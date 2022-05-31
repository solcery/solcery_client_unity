using System.Collections.Generic;

namespace Solcery.DebugViewers.States.Games.Objects
{
    public interface IObjectsValue
    {
        IReadOnlyList<string> ObjectKeys { get; }
        IReadOnlyList<IObjectValue> Objects { get; }
    }
}