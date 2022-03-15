namespace Solcery.Games.Contexts.GameStates
{
    public sealed class ContextGameStateDelay : ContextGameState
    {
        private int _delay;

        public static ContextGameState Create(int msec)
        {
            return new ContextGameStateDelay(msec);
        }
        
        private ContextGameStateDelay(int msec)
        {
            _delay = msec;
        }

        public bool CanDestroy(int msec)
        {
            _delay -= msec;
            return _delay <= 0;
        }
    }
}