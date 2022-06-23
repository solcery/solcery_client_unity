namespace Solcery.Games.States
{
    public sealed class PauseState : State
    {
        public int Delay => _delay;
        
        private int _delay;

        public static State Create(int delay)
        {
            return new PauseState(delay);
        }
        
        private PauseState(int delay)
        {
            _delay = delay;
        }

        public bool IsCompleted(int deltaTimeMsec)
        {
            _delay -= deltaTimeMsec;
            return _delay <= 0;
        }
    }
}