namespace Solcery.BrickInterpretation.Values
{
    public abstract class BrickValue : Brick<int>
    {
        protected BrickValue(int type, int subType) : base(type, subType) { }
    }
}