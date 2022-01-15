namespace Solcery.BrickInterpretation.Runtime.Contexts.Attrs
{
    public interface IContextGameAttrs
    {
        bool Contains(string attrName);
        void Update(string attrName, int attrValue);
        bool TryGetValue(string attrName, out int attrValue);
    }
}