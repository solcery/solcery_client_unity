namespace Solcery.Widgets_new.Eclipse.EcsSupport
{
    public interface IEntityId
    {
        int AttachEntityId { get; }
        void UpdateAttachEntityId(int entityId = -1);
    }
}