using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeGridCore.Code.Table
{
    public class TableData
    {
        public string ID { get; set; }
        public string Caption { get; set; }
        public bool ExpandFirstLevel { get; set; }
        public string Key { get; set; }
        public string Group { get; set; }
        public string OrderByColumn { get; set; }

        public List<TableColumn> Columns { get; } = new List<TableColumn>();
        public TableContent Data { get; } = new TableContent();
    }

    public class TableColumn
    {
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Type { get; set; }
        public int Precision { get; set; }
        public bool Hidden { get; set; }
        public int Width { get; set; }
        public string Align { get; set; }
        public List<TableColumn> Children { get; } = new List<TableColumn>();
    }

    public class TableContent
    {
        public string Type { get; set; }
        public List<string> Schema { get; } = new List<string>();
        public List<TableRow> Rows { get; } = new List<TableRow>();
    }

    public class TableRow : List<object>
    { }
}
