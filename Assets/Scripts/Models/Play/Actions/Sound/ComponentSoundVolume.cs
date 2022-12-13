using Leopotam.EcsLite;

namespace Solcery.Models.Play.Actions.Sound
{
    public struct ComponentSoundVolume : IEcsAutoReset<ComponentSoundVolume>
    {
        public int Volume;
        
        public void AutoReset(ref ComponentSoundVolume c)
        {
            c.Volume = 1;
        }
    }
}