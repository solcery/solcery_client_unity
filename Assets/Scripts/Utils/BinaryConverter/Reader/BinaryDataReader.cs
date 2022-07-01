using System;
using System.Text;
using Solcery.Utils.BinaryConverter.Writer;
using UnityEngine.Pool;

namespace Solcery.Utils.BinaryConverter.Reader
{
    public sealed class BinaryDataReader : IBinaryDataReader
    {
        private static readonly ObjectPool<IBinaryDataReader> Pool;

        static BinaryDataReader()
        {
            Pool = new ObjectPool<IBinaryDataReader>(Create, null, reader => reader.Clear());
        }

        private byte[] _data;
        private int _position;
        private int _dataSize;

        public byte[] Data => _data;
        public int Position => _position;
        public bool EndOfData => _position == _dataSize;
        public int AvailableBytes => _dataSize - _position;

        public void SetSource(BinaryDataWriter dataWriter)
        {
            _data = dataWriter.Data;
            _position = 0;
            _dataSize = dataWriter.Length;
        }

        public void SetSource(byte[] source)
        {
            _data = source;
            _position = 0;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset)
        {
            _data = source;
            _position = offset;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset, int dataSize)
        {
            _data = source;
            _position = offset;
            _dataSize = dataSize;
        }

        public static IBinaryDataReader Get()
        {
            return Pool.Get();
        }

        public static void Release(IBinaryDataReader binaryDataReader)
        {
            Pool.Release(binaryDataReader);
        }

        private static IBinaryDataReader Create()
        {
            return new BinaryDataReader();
        }

        private BinaryDataReader()
        {

        }

        #region GetMethods

        public byte GetByte()
        {
            var res = _data[_position];
            _position += 1;
            return res;
        }

        public sbyte GetSByte()
        {
            var b = (sbyte)_data[_position];
            _position++;
            return b;
        }

        public bool[] GetBoolArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new bool[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetBool();
            }
            return arr;
        }

        public ushort[] GetUShortArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new ushort[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetUShort();
            }
            return arr;
        }

        public short[] GetShortArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new short[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetShort();
            }
            return arr;
        }

        public long[] GetLongArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new long[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetLong();
            }
            return arr;
        }

        public ulong[] GetULongArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new ulong[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetULong();
            }
            return arr;
        }

        public int[] GetIntArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new int[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetInt();
            }
            return arr;
        }

        public uint[] GetUIntArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new uint[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetUInt();
            }
            return arr;
        }

        public float[] GetFloatArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new float[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetFloat();
            }
            return arr;
        }

        public double[] GetDoubleArray()
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new double[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetDouble();
            }
            return arr;
        }

        public string[] GetStringArray(int maxLength)
        {
            var size = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new string[size];
            for(var i = 0; i < size; i++)
            {
                arr[i] = GetString(maxLength);
            }
            return arr;
        }

        public bool GetBool()
        {
            var res = _data[_position] > 0;
            _position += 1;
            return res;
        }

        public ushort GetUShort()
        {
            var result = BitConverter.ToUInt16(_data, _position);
            _position += 2;
            return result;
        }

        public short GetShort()
        {
            var result = BitConverter.ToInt16(_data, _position);
            _position += 2;
            return result;
        }

        public long GetLong()
        {
            var result = BitConverter.ToInt64(_data, _position);
            _position += 8;
            return result;
        }

        public ulong GetULong()
        {
            var result = BitConverter.ToUInt64(_data, _position);
            _position += 8;
            return result;
        }

        public int GetInt()
        {
            var result = BitConverter.ToInt32(_data, _position);
            _position += 4;
            return result;
        }

        public uint GetUInt()
        {
            var result = BitConverter.ToUInt32(_data, _position);
            _position += 4;
            return result;
        }

        public float GetFloat()
        {
            var result = BitConverter.ToSingle(_data, _position);
            _position += 4;
            return result;
        }

        public double GetDouble()
        {
            var result = BitConverter.ToDouble(_data, _position);
            _position += 8;
            return result;
        }

        public string GetString(int maxLength)
        {
            var bytesCount = GetInt();
            if(bytesCount <= 0 || bytesCount > maxLength * 2)
            {
                return string.Empty;
            }

            var charCount = Encoding.UTF8.GetCharCount(_data, _position, bytesCount);
            if(charCount > maxLength)
            {
                return string.Empty;
            }

            var result = Encoding.UTF8.GetString(_data, _position, bytesCount);
            _position += bytesCount;
            return result;
        }

        public string GetString()
        {
            var bytesCount = GetInt();
            if(bytesCount <= 0)
            {
                return string.Empty;
            }

            var result = Encoding.UTF8.GetString(_data, _position, bytesCount);
            _position += bytesCount;
            return result;
        }

        public byte[] GetRemainingBytes()
        {
            var outgoingData = new byte[AvailableBytes];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, AvailableBytes);
            _position = _data.Length;
            return outgoingData;
        }

        public void GetRemainingBytes(byte[] destination)
        {
            Buffer.BlockCopy(_data, _position, destination, 0, AvailableBytes);
            _position = _data.Length;
        }

        public void GetBytes(byte[] destination, int lenght)
        {
            Buffer.BlockCopy(_data, _position, destination, 0, lenght);
            _position += lenght;
        }

        public byte[] GetBytesWithLength()
        {
            var length = GetInt();
            var outgoingData = new byte[length];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, length);
            _position += length;
            return outgoingData;
        }
        #endregion

        #region PeekMethods

        public byte PeekByte()
        {
            return _data[_position];
        }

        public sbyte PeekSByte()
        {
            return (sbyte)_data[_position];
        }

        public bool PeekBool()
        {
            return _data[_position] > 0;
        }

        public ushort PeekUShort()
        {
            return BitConverter.ToUInt16(_data, _position);
        }

        public short PeekShort()
        {
            return BitConverter.ToInt16(_data, _position);
        }

        public long PeekLong()
        {
            return BitConverter.ToInt64(_data, _position);
        }

        public ulong PeekULong()
        {
            return BitConverter.ToUInt64(_data, _position);
        }

        public int PeekInt()
        {
            return BitConverter.ToInt32(_data, _position);
        }

        public uint PeekUInt()
        {
            return BitConverter.ToUInt32(_data, _position);
        }

        public float PeekFloat()
        {
            return BitConverter.ToSingle(_data, _position);
        }

        public double PeekDouble()
        {
            return BitConverter.ToDouble(_data, _position);
        }

        public string PeekString(int maxLength)
        {
            var bytesCount = BitConverter.ToInt32(_data, _position);
            if(bytesCount <= 0 || bytesCount > maxLength * 2)
            {
                return string.Empty;
            }

            var charCount = Encoding.UTF8.GetCharCount(_data, _position + 4, bytesCount);
            if(charCount > maxLength)
            {
                return string.Empty;
            }

            var result = Encoding.UTF8.GetString(_data, _position + 4, bytesCount);
            return result;
        }

        public string PeekString()
        {
            var bytesCount = BitConverter.ToInt32(_data, _position);
            if(bytesCount <= 0)
            {
                return string.Empty;
            }

            var result = Encoding.UTF8.GetString(_data, _position + 4, bytesCount);
            return result;
        }
        #endregion

        public byte[] CopyData(int offset, int size)
        {
            var resultData = new byte[size];

            if(size > 0)
                Buffer.BlockCopy(_data, 0, resultData, offset, size);

            return resultData;
        }

        public void Clear()
        {
            _position = 0;
            _dataSize = 0;
            _data = null;
        }
    }
}
