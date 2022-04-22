using Solcery.DebugViewers.States.Games.Attrs;

namespace Solcery.DebugViewers.States.Games.Objects
{
    public interface IObjectValue
    {
        bool IsDestroyed { get; }
        bool IsCreated { get; }
        int Id { get; }
        int TplId { get; }
        IAttrsValue Attrs { get; }
    }
}