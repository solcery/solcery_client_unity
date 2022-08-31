namespace Solcery.Services.Sound
{
    public interface IServiceSound
    {
        void Play(int soundId);
        void Cleanup();
    }
}