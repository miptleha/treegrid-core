using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeGridCore.Models
{
    public class FileXml
    {
        public string Path { get; set; }
        public string ShortPath
        {
            get
            {
                return System.IO.Path.GetFileName(Path);
            }
        }
    }
}
