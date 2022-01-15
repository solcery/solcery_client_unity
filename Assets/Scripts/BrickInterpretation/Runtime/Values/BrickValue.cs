namespace Solcery.BrickInterpretation.Runtime.Values
{
    public abstract class BrickValue : Brick<int>
    {
        protected BrickValue(int type, int subType) : base(type, subType) { }
    }
}