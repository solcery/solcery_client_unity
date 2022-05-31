namespace Solcery.Models.Shared.Objects
{
    public interface IObjectIdHash
    {
        void UpdateHeadId(int maxObjectId);
        void Reset();
        int GetId();
        void ReleaseId(int objectId);
    }
}