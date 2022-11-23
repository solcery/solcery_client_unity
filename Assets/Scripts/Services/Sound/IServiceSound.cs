namespace Solcery.Services.Sound
{
    public interface IServiceSound
    {
        void Play(int soundId, int volume);
        void Cleanup();
        void Destroy();
    }
}