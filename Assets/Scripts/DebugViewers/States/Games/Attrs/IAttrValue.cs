namespace Solcery.DebugViewers.States.Games.Attrs
{
    public interface IAttrValue
    {
        bool IsChange { get; }
        string Key { get; }
        int CurrentValue { get; }
        int OldValue { get; }
    }
}