using Leopotam.EcsLite;

namespace Solcery.Models.Play.Actions.Sound
{
    public struct ComponentSoundId : IEcsAutoReset<ComponentSoundId>
    {
        public int SoundId;
        
        public void AutoReset(ref ComponentSoundId c)
        {
            c.SoundId = -1;
        }
    }
}