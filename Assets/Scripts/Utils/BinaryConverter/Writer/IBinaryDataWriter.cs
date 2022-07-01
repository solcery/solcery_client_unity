namespace Solcery.Utils.BinaryConverter.Writer
{
    public interface IBinaryDataWriter
    {
        void ResizeIfNeed(int newSize);
        void Reset(int size);
        void Reset();
        void Cleanup();
        byte[] CopyData();
        byte[] Data { get; }
        int Length { get; }
        void Put(float value);
        void Put(double value);
        void Put(long value);
        void Put(ulong value);
        void Put(int value);
        void Put(uint value);
        void Put(ushort value);
        void Put(short value);
        void Put(sbyte value);
        void Put(byte value);
        void Put(byte[] data, int offset, int length);
        void Put(byte[] data);
        void Put(bool value);
        void Put(string value);
        void Put(string value, int maxLength);
        void PutBytesWithLength(byte[] data, int offset, int length);
        void PutBytesWithLength(byte[] data);
        void PutArray(float[] value);
        void PutArray(double[] value);
        void PutArray(long[] value);
        void PutArray(ulong[] value);
        void PutArray(int[] value);
        void PutArray(uint[] value);
        void PutArray(ushort[] value);
        void PutArray(short[] value);
        void PutArray(bool[] value);
        void PutArray(string[] value);
        void PutArray(string[] value, int maxLength);
    }
}