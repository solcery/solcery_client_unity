namespace Solcery.Games.States.New.Actions.Animation
{
    public abstract class UpdateActionAnimation : UpdateAction
    {
        protected UpdateActionAnimation(int stateId) : base(stateId) { }
        
        public abstract int GetDurationMsec();
    }
}