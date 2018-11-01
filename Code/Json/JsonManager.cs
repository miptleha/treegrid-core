using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGridCore.Code.Json
{
    public class JsonManager
    {
        public static object ReadJson(string path)
        {
            if (!File.Exists(path))
                return null;

            object res = null;
            using (var fs = File.OpenRead(path))
            {
                res = ReadJson(fs);
            }
            return res;
        }

        public static List<Dictionary<string, object>> ReadJson(Stream s)
        {
            List<Dictionary<string, object>> res = null;
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(s))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    res = serializer.Deserialize<List<Dictionary<string, object>>>(jsonTextReader);
                }
            }
            return res;
        }

        public static List<Dictionary<string, object>> ReadJsonString(string content)
        {
            using (var st = GenerateStreamFromString(content))
            {
                return ReadJson(st);
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
