namespace Solcery.BrickInterpretation.Runtime.Conditions
{
    public abstract class BrickCondition : Brick<bool>
    {
        protected BrickCondition(int type, int subType) : base(type, subType) { }
    }
}