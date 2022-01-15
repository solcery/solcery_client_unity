namespace Solcery.BrickInterpretation.Runtime.Contexts.Objects
{
    public interface IContextObject
    {
        void Push(object @object);
        bool TryPop<T>(out T @object);
        bool TryPeek<T>(out T @object);
    }
}