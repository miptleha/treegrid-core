using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGridCore.Code.Csv
{
    class CsvManager
    {
        public static TableData ReadCsv(string path)
        {
            if (!File.Exists(path))
                return null;

            using (var fs = File.OpenRead(path))
            {
                return ReadCsv(fs);
            }
        }

        public static void WriteCsv(string path, TableData data)
        {
            using (var fs = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                WriteCsv(fs, data);
            }
        }

        public static TableData ReadCsv(Stream ms)
        {
            var data = new TableData();
            StreamReader sr = new StreamReader(ms, Encoding.UTF8);
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;
                data.Header.AddRange(s.Split(new string[] { "; " }, StringSplitOptions.None));
                break;
            }
            while ((s = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;
                data.Content.Add(s.Split(new string[] { "; " }, StringSplitOptions.None));
            }
            return data;
        }

        public static void WriteCsv(Stream ms, TableData data)
        {
            StreamWriter sw = new StreamWriter(ms, Encoding.UTF8);
            sw.WriteLine(String.Join("; ", data.Header));
            foreach (var r in data.Content)
            {
                sw.WriteLine(String.Join("; ", r));
            }
            sw.Flush();
        }

        public static TableData ReadCsvString(string content)
        {
            using (var st = GenerateStreamFromString(content))
            {
                return ReadCsv(st);
            }
        }

        public static string WriteCsvString(TableData data)
        {
            using (var ms = new MemoryStream())
            {
                WriteCsv(ms, data);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
