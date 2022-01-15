namespace Solcery.BrickInterpretation.Runtime.Contexts.Objects
{
    public interface IContextObjectAttrs
    {
        bool Contains(object contextObject, string attrName);
        void Update(object contextObject, string attrName, int attrValue);
        bool TryGetValue(object contextObject, string attrName, out int attrValue);
    }
}