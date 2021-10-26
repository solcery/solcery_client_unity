namespace Solcery.BrickInterpretation
{
    public interface IBrickService
    {
        void Registration(Brick brick);
        void Cleanup();
        bool TryCreate<T>(string brickTypeName, out T brick) where T : Brick;
        void Free(Brick brick);
    }
}