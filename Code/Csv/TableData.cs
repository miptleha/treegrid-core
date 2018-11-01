using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeGridCore.Code.Csv
{
    class TableData
    {
        public TableData()
        {
            Header = new TableHeader();
            Content = new TableContent();
        }

        public TableHeader Header { get; set; }
        public TableContent Content { get; set; }
    }

    class TableHeader : List<string>
    {

    }

    class TableContent : List<TableRow>
    {
        public void Add(string[] row)
        {
            var r = new TableRow();
            r.AddRange(row);
            Add(r);
        }
    }

    class TableRow : List<string>
    {

    }
}
