using System.Collections.Generic;

namespace Solcery.Models.Shared.Objects
{
    public class ObjectIdHash : IObjectIdHash
    {
        private readonly Stack<int> _cache;
        private int _headId;

        public static IObjectIdHash Create()
        {
            return new ObjectIdHash();
        }

        private ObjectIdHash()
        {
            _cache = new Stack<int>();
            _headId = 0;
        }

        void IObjectIdHash.UpdateHeadId(int maxObjectId)
        {
            _headId = maxObjectId + 1;
        }

        void IObjectIdHash.Reset()
        {
            _cache.Clear();
            _headId = 0;
        }

        int IObjectIdHash.GetId()
        {
            if (_cache.Count > 0)
            {
                return _cache.Pop();
            }

            ++_headId;
            return _headId;
        }

        void IObjectIdHash.ReleaseId(int objectId)
        {
            _cache.Push(objectId);
        }
    }
}