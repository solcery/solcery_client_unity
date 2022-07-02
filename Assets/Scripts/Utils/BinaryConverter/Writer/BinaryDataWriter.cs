using System;
using System.Text;
using UnityEngine.Pool;

namespace Solcery.Utils.BinaryConverter.Writer
{
    public sealed class BinaryDataWriter : IBinaryDataWriter
    {
        private static readonly ObjectPool<IBinaryDataWriter> Pool;

        static BinaryDataWriter()
        {
            Pool = new ObjectPool<IBinaryDataWriter>(Create, null, writer => writer.Cleanup());
        }
        
        private byte[] _data;
        private int _position;

        private int _maxLength;
        private readonly bool _autoResize;
        
        public static IBinaryDataWriter Get()
        {
            return Pool.Get();
        }

        public static void Release(IBinaryDataWriter binaryDataWriter)
        {
            Pool.Release(binaryDataWriter);
        }

        private static IBinaryDataWriter Create()
        {
            return new BinaryDataWriter();
        }

        private BinaryDataWriter()
        {
            _maxLength = 64;
            _data = new byte[_maxLength];
            _autoResize = true;
        }

        public void ResizeIfNeed(int newSize)
        {
            if(_maxLength < newSize)
            {
                while(_maxLength < newSize)
                {
                    _maxLength *= 2;
                }
                Array.Resize(ref _data, _maxLength);
            }
        }

        public void Reset(int size)
        {
            ResizeIfNeed(size);
            _position = 0;
        }

        public void Reset()
        {
            _position = 0;
        }

        public void Cleanup()
        {
            Reset();
            _maxLength = 64;
            _data = new byte[_maxLength];
        }

        public byte[] CopyData()
        {
            var resultData = new byte[_position];
            Buffer.BlockCopy(_data, 0, resultData, 0, _position);
            return resultData;
        }

        public byte[] Data => _data;
        public int Length => _position;

        public void Put(float value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(double value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(long value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(ulong value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Put(int value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(uint value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Put(ushort value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Put(short value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Put(sbyte value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = (byte)value;
            _position++;
        }

        public void Put(byte value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = value;
            _position++;
        }

        public void Put(byte[] data, int offset, int length)
        {
            if(_autoResize)
                ResizeIfNeed(_position + length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }

        public void Put(byte[] data)
        {
            if(_autoResize)
                ResizeIfNeed(_position + data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }

        public void PutBytesWithLength(byte[] data, int offset, int length)
        {
            if(_autoResize)
                ResizeIfNeed(_position + length);
            Put(length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }

        public void PutBytesWithLength(byte[] data)
        {
            if(_autoResize)
                ResizeIfNeed(_position + data.Length);
            Put(data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }

        public void Put(bool value)
        {
            if(_autoResize)
                ResizeIfNeed(_position + 1);
            _data[_position] = (byte)(value ? 1 : 0);
            _position++;
        }

        public void PutArray(float[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len * 4 + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(double[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len * 8 + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(long[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len * 8 + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(ulong[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len * 8 + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(int[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len * 4 + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(uint[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len * 4 + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(ushort[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len * 2 + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(short[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len * 2 + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(bool[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            if(_autoResize)
                ResizeIfNeed(_position + len + 2);
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(string[] value)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            Put(len);
            for(var i = 0; i < value.Length; i++)
            {
                Put(value[i]);
            }
        }

        public void PutArray(string[] value, int maxLength)
        {
            var len = value == null ? (ushort)0 : (ushort)value.Length;
            Put(len);
            for(var i = 0; i < len; i++)
            {
                Put(value[i], maxLength);
            }
        }

        public void Put(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                Put(0);
                return;
            }

            //put bytes count
            var bytesCount = Encoding.UTF8.GetByteCount(value);
            if(_autoResize)
                ResizeIfNeed(_position + bytesCount + 4);
            Put(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, value.Length, _data, _position);
            _position += bytesCount;
        }

        public void Put(string value, int maxLength)
        {
            if(string.IsNullOrEmpty(value))
            {
                Put(0);
                return;
            }

            var length = value.Length > maxLength ? maxLength : value.Length;
            //calculate max count
            var bytesCount = Encoding.UTF8.GetByteCount(value);
            if(_autoResize)
                ResizeIfNeed(_position + bytesCount + 4);

            //put bytes count
            Put(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, length, _data, _position);

            _position += bytesCount;
        }
    }
}
