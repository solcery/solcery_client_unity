using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Solcery.Utils.BinaryConverter.Reader;
using Solcery.Utils.BinaryConverter.Writer;

namespace Solcery.DebugViewers.StateQueues.Binary
{
    public abstract class DebugUpdateStateBinary
    {
        public void InitFromJson(JObject value)
        {
            FromJsonImpl(value);
        }

        protected abstract void FromJsonImpl(JObject value);

        public void ReadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            using var file = File.OpenRead(path);
            var buffer = new Span<byte>();
            file.Read(buffer);
            var reader = BinaryDataReader.Get();
            reader.SetSource(buffer.ToArray());
            FromBinaryImpl(reader);
            BinaryDataReader.Release(reader);
            buffer.Clear();
        }

        protected abstract void FromBinaryImpl(IBinaryDataReader reader);

        public void WriteForFile(string path)
        {
            using var file = File.OpenWrite(path);
            var writer = BinaryDataWriter.Get();
            ToBinaryImpl(writer);
            file.Write(writer.Data);
            BinaryDataWriter.Release(writer);
        }

        protected abstract void ToBinaryImpl(IBinaryDataWriter writer);

        public void Cleanup()
        {
            CleanupImpl();
        }

        protected abstract void CleanupImpl();
    }
}