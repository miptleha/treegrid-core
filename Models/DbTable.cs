using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeGridCore.Models
{
    public class DbTable
    {
        public DbTableSchema Schema { get { return _schema; } }
        public DbTableData Data { get { return _data; } }

        DbTableSchema _schema = new DbTableSchema();
        DbTableData _data = new DbTableData();
    }

    public class DbTableData : List<DbTableDataRow>
    {
    }

    public class DbTableDataRow : List<object>
    {
        public DbTableDataRow(int n)
            : base(n)
        {
            for (int i = 0; i < n; i++)
                this.Add("");
        }
    }

    public class DbTableSchema : List<string>
    {
    }
}
