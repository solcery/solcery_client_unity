namespace Solcery.BrickInterpretation.Runtime.Conditions
{
    public enum BrickConditionTypes
    {
        Constant = 0,
        Not,
        Equal = 2,
        GreaterThan = 3,
        LesserThan = 4,
        Argument = 5,
        Or = 6,
        And = 7
    }
}