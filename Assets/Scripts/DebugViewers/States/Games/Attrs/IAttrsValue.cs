using System.Collections.Generic;

namespace Solcery.DebugViewers.States.Games.Attrs
{
    public interface IAttrsValue
    {
        IReadOnlyList<IAttrValue> Attrs { get; }
    }
}