using System.Collections.Generic;
using Solcery.Services.GameContent;
using Solcery.Services.Sound.Play;

namespace Solcery.Services.Sound
{
    public sealed class ServiceSound : IServiceSound
    {
        private readonly Stack<SoundPlayController> _stack;
        private readonly SoundsLayout _layout;
        private readonly IServiceGameContent _serviceGameContent;
        
        public static IServiceSound Create(SoundsLayout layout, IServiceGameContent gameContent)
        {
            return new ServiceSound(layout, gameContent);
        }

        private ServiceSound(SoundsLayout layout, IServiceGameContent gameContent)
        {
            _layout = layout;
            _serviceGameContent = gameContent;
        }

        void IServiceSound.Play(int soundId)
        {
            
        }

        void IServiceSound.Cleanup()
        {
            
        }
    }
}