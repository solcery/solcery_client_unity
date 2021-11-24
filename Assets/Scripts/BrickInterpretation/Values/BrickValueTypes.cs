namespace Solcery.BrickInterpretation.Values
{
    public enum BrickValueTypes
    {
        Const = 0,
        Variable,
        Attribute,
        Argument,
        IfThenElse,
        Add,
        Sub,
        Div,
        Mod,
        Random = 10,
        CardId,
        CardTypeId
    }
}