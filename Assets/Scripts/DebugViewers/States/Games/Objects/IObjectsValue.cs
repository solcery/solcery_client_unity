using System.Collections.Generic;

namespace Solcery.DebugViewers.States.Games.Objects
{
    public interface IObjectsValue
    {
        IReadOnlyList<IObjectValue> Objects { get; }
    }
}