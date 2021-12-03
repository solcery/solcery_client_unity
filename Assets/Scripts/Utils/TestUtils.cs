using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Solcery.Utils
{
    public sealed class TestUtils
    {
        private static readonly Lazy<TestUtils> Lazy = 
            new Lazy<TestUtils>(() => new TestUtils());

        private static TestUtils Instantiate => Lazy.Value;

        private List<Tuple<int, string>> _lines = new List<Tuple<int, string>>();

        public static void AddLine(int level, string line)
        {
            Instantiate._lines.Add(new Tuple<int, string>(level, line));
        }

        public static void WriteAllLines()
        {
            var path = Path.GetFullPath($"{Application.dataPath}/Tests/Logs/log_{DateTime.Now.ToString(CultureInfo.InvariantCulture)}");

            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var sb = new StringBuilder();
            using (var file = File.CreateText(path))
            {
                foreach (var line in Instantiate._lines)
                {
                    for (var i = 0; i < line.Item1; i++)
                    {
                        sb.Append('\t');
                    }

                    sb.Append(line.Item2);
                    sb.Append('\n');
                }
                
                file.WriteLine(sb);
            }
        }
    }
}