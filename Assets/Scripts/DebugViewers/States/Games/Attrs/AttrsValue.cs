using System.Collections.Generic;
using Solcery.DebugViewers.StateQueues.Binary.Game.Attr;

namespace Solcery.DebugViewers.States.Games.Attrs
{
    public sealed class AttrsValue : IAttrsValue
    {
        IReadOnlyList<IAttrValue> IAttrsValue.Attrs => _attrs;

        private readonly List<IAttrValue> _attrs;

        public static IAttrsValue Create(IReadOnlyList<IDUGSBAttrValue> attrs)
        {
            return new AttrsValue(attrs);
        }

        private AttrsValue(IReadOnlyList<IDUGSBAttrValue> attrs)
        {
            _attrs = new List<IAttrValue>();
            foreach (var attr in attrs)
            {
                _attrs.Add(AttrValue.Create(attr.Key, attr.Current, attr.Preview));
            }
        }
    }
}