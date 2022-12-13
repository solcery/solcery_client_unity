namespace Solcery.Games.States.New.Actions.Animation
{
    public abstract class UpdateActionAnimation : UpdateAction
    {
        public int DelayMSec => _delayMSec;
        
        private int _delayMSec;

        protected UpdateActionAnimation(int stateId) : base(stateId)
        {
            _delayMSec = 0;
        }
        
        public abstract int GetDurationMsec();

        public void UpdateDelay(int delayMSec)
        {
            _delayMSec = delayMSec;
        }
    }
}