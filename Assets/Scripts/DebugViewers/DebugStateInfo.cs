namespace Solcery.DebugViewers
{
    public sealed class DebugStateInfo
    {
        public DebugStateTypes Type => _type;
        public int Index => _index;
        
        private readonly DebugStateTypes _type;
        private readonly int _index;

        public static DebugStateInfo Create(DebugStateTypes type, int index)
        {
            return new DebugStateInfo(type, index);
        }

        private DebugStateInfo(DebugStateTypes type, int index)
        {
            _type = type;
            _index = index;
        }
    }
}