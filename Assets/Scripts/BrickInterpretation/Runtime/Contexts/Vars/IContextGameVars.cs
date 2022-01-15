namespace Solcery.BrickInterpretation.Runtime.Contexts.Vars
{
    public interface IContextGameVars
    {
        void Update(string varName, int varValue);
        bool TryGet(string varName, out int varValue);
    }
}