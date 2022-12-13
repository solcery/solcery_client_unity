using Leopotam.EcsLite;
using Solcery.Games.States.New.Actions;
using Solcery.Games.States.New.Actions.Animation.Sequence;
using Solcery.Models.Play.Actions.Factory;

namespace Solcery.Models.Play.Actions.Animation.Sequence
{
    public static class CreatorAnimationSequence
    {
        public static bool Create(IActionObjectFactory factory, EcsWorld world, UpdateAction action)
        {
            if (action is UpdateActionAnimationSequence actionAnimationSequence)
            {
                foreach (var actionAnimation in actionAnimationSequence.Actions)
                {
                    factory.ApplyAction(factory, world, actionAnimation);
                }
                return true;
            }

            return false;
        }
    }
}