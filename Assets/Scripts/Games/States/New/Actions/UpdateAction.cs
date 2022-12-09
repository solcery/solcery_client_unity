namespace Solcery.Games.States.New.Actions
{
    public abstract class UpdateAction
    {
        public int StateId { get; }

        protected UpdateAction(int stateId)
        {
            StateId = stateId;
        }
    }
}