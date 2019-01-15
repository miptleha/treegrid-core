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
        static CsvManager()
        {
            Separator = ';';
            TextEncoding = "UTF-8";
            TextTrim = false;
        }

        public static char Separator { get; set; }
        public static string TextEncoding { get; set; }
        public static bool TextTrim { get; set; }

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
            StreamReader sr = new StreamReader(ms, Encoding.GetEncoding(TextEncoding));
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;
                data.Header.AddRange(ReadRow(s));
                break;
            }
            while ((s = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(s))
                    continue;
                data.Content.Add(ReadRow(s));
            }
            return data;
        }

        private static string[] ReadRow(string text)
        {
            //simple version
            //return text.Split(new string[] { Separator }, StringSplitOptions.None);

            //https://stackoverflow.com/questions/8493195/how-can-i-parse-a-csv-string-with-javascript-which-contains-comma-in-data
            var p = "";
            var row = new List<string>();
            var i = 0;
            var s = true;
            foreach (var l1 in text)
            {
                var l = new string(l1, 1);
                if ("\"" == l)
                {
                    if (s && l == p)
                    {
                        while (row.Count <= i)
                            row.Add("");
                        row[i] += l;
                    }
                    s = !s;
                }
                else if (Separator.ToString() == l && s)
                {
                    i++;
                    while (row.Count <= i)
                        row.Add("");
                    l = row[i] = "";
                }
                else
                {
                    while (row.Count <= i)
                        row.Add("");
                    row[i] += l;
                }
                p = l;
            }
            if (TextTrim)
                row = row.Select(v => v.Trim()).ToList();
            return row.ToArray();
        }

        public static void WriteCsv(Stream ms, TableData data)
        {
            StreamWriter sw = new StreamWriter(ms, Encoding.GetEncoding(TextEncoding));
            sw.WriteLine(WriteRow(data.Header));
            foreach (var r in data.Content)
            {
                sw.WriteLine(WriteRow(r));
            }
            sw.Flush();
        }

        private static string WriteRow(List<string> row)
        {
            //simple version
            //return String.Join(Separator, row);

            var r1 = new List<string>();
            foreach (var v in row)
            {
                var v1 = v;
                if (v1 == null)
                    v1 = "";
                v1 = v1.Replace("\"", "\"\"");
                v1 = v1.Replace("\r", "");
                v1 = v1.Replace("\n", " ");
                if (v1.Contains("\"") || v1.Contains(Separator.ToString()))
                    v1 = "\"" + v1 + "\"";
                r1.Add(v1);
            }
            return String.Join(Separator.ToString(), r1);
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
