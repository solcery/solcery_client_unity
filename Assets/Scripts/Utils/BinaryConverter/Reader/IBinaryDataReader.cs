using Solcery.Utils.BinaryConverter.Writer;

namespace Solcery.Utils.BinaryConverter.Reader
{
    public interface IBinaryDataReader
    {
        byte[] Data { get; }
        int Position { get; }
        bool EndOfData { get; }
        int AvailableBytes { get; }
        void SetSource(BinaryDataWriter dataWriter);
        void SetSource(byte[] source);
        void SetSource(byte[] source, int offset);
        void SetSource(byte[] source, int offset, int dataSize);
        byte GetByte();
        sbyte GetSByte();
        bool[] GetBoolArray();
        ushort[] GetUShortArray();
        short[] GetShortArray();
        long[] GetLongArray();
        ulong[] GetULongArray();
        int[] GetIntArray();
        uint[] GetUIntArray();
        float[] GetFloatArray();
        double[] GetDoubleArray();
        string[] GetStringArray(int maxLength);
        bool GetBool();
        ushort GetUShort();
        short GetShort();
        long GetLong();
        ulong GetULong();
        int GetInt();
        uint GetUInt();
        float GetFloat();
        double GetDouble();
        string GetString(int maxLength);
        string GetString();
        byte[] GetRemainingBytes();
        void GetRemainingBytes(byte[] destination);
        void GetBytes(byte[] destination, int lenght);
        byte[] GetBytesWithLength();
        byte PeekByte();
        sbyte PeekSByte();
        bool PeekBool();
        ushort PeekUShort();
        short PeekShort();
        long PeekLong();
        ulong PeekULong();
        int PeekInt();
        uint PeekUInt();
        float PeekFloat();
        double PeekDouble();
        string PeekString(int maxLength);
        string PeekString();
        byte[] CopyData(int offset, int size);
        void Clear();
    }
}