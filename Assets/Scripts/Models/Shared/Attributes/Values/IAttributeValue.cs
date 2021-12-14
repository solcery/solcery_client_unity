namespace Solcery.Models.Shared.Attributes.Values
{
    public interface IAttributeValue
    {
        int Old { get; }
        int Current { get; }
        bool Changed { get; }
        void UpdateValue(int value);
        void Cleanup();
    }
}