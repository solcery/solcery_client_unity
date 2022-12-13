using Leopotam.EcsLite;
using Solcery.Games.States.New.Actions;
using Solcery.Games.States.New.Actions.PlaySound;
using Solcery.Models.Play.Actions.Factory;

namespace Solcery.Models.Play.Actions.Sound
{
    public static class CreatorSound
    {
        public static bool Create(IActionObjectFactory factory, EcsWorld world, UpdateAction action)
        {
            if (action is UpdateActionPlaySound actionPlaySound)
            {
                var entityId = world.NewEntity();
                world.GetPool<ComponentSoundTag>().Add(entityId);
                world.GetPool<ComponentSoundId>().Add(entityId).SoundId = actionPlaySound.SoundId;
                world.GetPool<ComponentSoundVolume>().Add(entityId).Volume = actionPlaySound.Volume;
                return true;
            }

            return false;
        }
    }
}