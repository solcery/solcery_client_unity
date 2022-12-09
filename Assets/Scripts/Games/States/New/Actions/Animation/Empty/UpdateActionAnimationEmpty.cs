namespace Solcery.Games.States.New.Actions.Animation.Empty
{
    public sealed class UpdateActionAnimationEmpty : UpdateActionAnimation
    {
        public static UpdateActionAnimation Create()
        {
            return new UpdateActionAnimationEmpty(-1);
        }
        
        private UpdateActionAnimationEmpty(int stateId) : base(stateId) { }
        
        public override int GetDurationMsec()
        {
            return 0;
        }
    }
}