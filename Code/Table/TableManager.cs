using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TreeGridCore.Code.Csv;
using TreeGridCore.Code.Json;

namespace TreeGridCore.Code.Table
{
    public class TableManager
    {
        string _file;

        public string GetXmlPath(string id, string path)
        {
            var files = Directory.GetFiles(path, "*.xml");
            XElement root = null;
            foreach (var f in files)
            {
                var doc = XDocument.Load(f);
                var table = doc.Element("table");
                if (table == null)
                    continue;
                var idAt = table.Attribute("id");
                if (idAt == null)
                    continue;
                if (idAt.Value.ToLower() == id.ToLower())
                {
                    root = table;
                    _file = f;
                    break;
                }
            }

            if (root == null)
                throw new Exception($"Table with id={id} not found in wwwroot/App_Data");

            return _file;
        }

        public TableData LoadTable(string id, string orderBy, string parentId, string path)
        {
            var files = Directory.GetFiles(path, "*.xml");
            XElement root = null;
            foreach (var f in files)
            {
                var doc = XDocument.Load(f);
                var table = doc.Element("table");
                if (table == null)
                    continue;
                var idAt = table.Attribute("id");
                if (idAt == null)
                    continue;
                if (idAt.Value.ToLower() == id.ToLower())
                {
                    root = table;
                    _file = f;
                    break;
                }
            }

            if (root == null)
                throw new Exception($"Table with id={id} not found in wwwroot/App_Data");

            var td = new TableData();
            td.ID = ReadAttr(root, "id");
            td.Caption = ReadAttr(root, "caption", true);
            td.ExpandFirstLevel = ReadBoolAttr(root, "expand", false);
            td.Key = ReadElem(root, "key", true);
            td.Group = ReadElem(root, "group", true);

            var columns = root.Element("columns");
            if (columns == null)
                throw new Exception($"Columns not found in {_file}");
            ReadColumns(columns, td.Columns);

            var data = root.Element("data");
            if (data == null)
                throw new Exception($"Data not found in {_file}");
            ReadData(data, td, orderBy, parentId);

            return td;
        }

        string ReadAttr(XElement e, string name, bool opt = false)
        {
            var a = e.Attribute(name);
            if (a == null)
            {
                if (opt)
                    return null;
                throw new Exception($"Attribute {name} not found in element {e.Name.LocalName} in {_file}");
            }
            return a.Value;
        }

        bool ReadBoolAttr(XElement e, string name, bool def)
        {
            var v = ReadAttr(e, name, true);
            if (v == null)
                return def;
            if (v != "0" && v != "1")
                throw new Exception($"Invalid value {v} (exprect 0 or 1) for boolean attribute {name} in element {e.Name.LocalName} in {_file}");
            return v == "1";
        }

        int ReadIntAttr(XElement e, string name, int def)
        {
            var v = ReadAttr(e, name, true);
            if (v != null)
            {
                int res;
                if (!int.TryParse(v, out res))
                    throw new Exception($"Invalid integer value {v} for attribute {name} in element {e.Name.LocalName} in {_file}");
                return res;
            }
            return def;
        }

        string ReadElem(XElement e, string name, bool opt = false)
        {
            var c = e.Element(name);
            if (c == null)
            {
                if (opt)
                    return null;
                throw new Exception($"Element {name} not found in element {e.Name.LocalName} in {_file}");
            }
            return c.Value;
        }

        readonly static string[] _types = { "string", "number", "date", "bool", "percent" };
        readonly static string[] _numericTypes = { "number", "percent" };
        const string _defaultType = "string";
        const int _defaultPrecision = 0;
        readonly static string[] _alignList = { "left", "right", "center" };

        void ReadColumns(XElement e, List<TableColumn> columns, TableColumn root = null)
        {
            foreach (var c in e.Elements("col"))
            {
                var tc = new TableColumn();
                tc.Name = ReadAttr(c, "name", true);
                tc.Caption = ReadAttr(c, "caption", true) ?? tc.Name;
                tc.Type = ReadAttr(c, "type", true) ?? (root != null ? root.Type : null) ?? _defaultType;
                if (!_types.Contains(tc.Type))
                    throw new Exception($"Not supported type {tc.Type} for column {tc.Name} in {_file}, list of supported types: {string.Join(", ", _types)}");
                tc.Precision = ReadIntAttr(c, "precision", -1);
                if (tc.Precision == -1 && root != null)
                    tc.Precision = root.Precision;
                if (tc.Precision == -1)
                    tc.Precision = _defaultPrecision;
                tc.Hidden = ReadBoolAttr(c, "hidden", false);
                tc.Width = ReadIntAttr(c, "width", 0);
                tc.Align = ReadAttr(c, "align", true) ?? (root != null ? root.Align : null);
                if (!string.IsNullOrEmpty(tc.Align) && !_alignList.Contains(tc.Align))
                    throw new Exception($"Not supported align {tc.Align} for column {tc.Name} in {_file}, list of supported aligns: {string.Join(", ", _alignList)}");

                ReadColumns(c, tc.Children, tc);
                if (tc.Children.Count == 0 && tc.Name == null)
                    throw new Exception($"For column without visible children elements name attribute must be specified in {_file}");
                if (tc.Children.Count > 0 && tc.Caption == null)
                    throw new Exception($"For column with children elements caption attribute must be specified in {_file}");

                columns.Add(tc);
            }
        }

        void ReadData(XElement x, TableData td, string orderBy, string parentId)
        {
            TableContent d = td.Data;

            string type = ReadAttr(x, "type");
            if (type == "json")
                ReadDataJson(x.Value, d);
            else if (type == "csv")
                ReadDataCsv(x.Value, d);
            else
                throw new Exception($"Inknown data type {type} in {_file}");

            Sort(td, orderBy);
            if (!string.IsNullOrEmpty(td.Group))
            {
                if (string.IsNullOrEmpty(td.Key))
                    throw new Exception("'key' element not set inside 'table'");
                Group(td, parentId);
            }
        }

        private void Group(TableData td, string parentId)
        {
            int keyCol = -1;
            int groupCol = -1;
            for (int i = 0; i < td.Data.Schema.Count; i++)
            {
                if (td.Key.ToLower() == td.Data.Schema[i].ToLower())
                    keyCol = i;
                if (td.Group.ToLower() == td.Data.Schema[i].ToLower())
                    groupCol = i;
            }
            if (keyCol == -1)
                throw new Exception("Key column not found in data");
            if (groupCol == -1)
                throw new Exception("Group column not found in data");

            var htRows = new Dictionary<string, List<TableRow>>();
            foreach (var r in td.Data.Rows)
            {
                var key = (r[groupCol] ?? "0").ToString();
                if (!htRows.ContainsKey(key))
                    htRows.Add(key, new List<TableRow>());
                htRows[key].Add(r);
            }

            var filtred = new List<TableRow>();
            if (string.IsNullOrEmpty(parentId))
            {
                //no parent
                foreach (var group in htRows.Keys)
                {
                    bool found = false;
                    foreach (var r in td.Data.Rows)
                    {
                        var key = (r[keyCol] ?? "0").ToString();
                        if (key == group)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        filtred.AddRange(htRows[group]);
                }
            }
            else
            {
                var key = parentId;
                if (htRows.ContainsKey(key))
                    filtred = htRows[key];
            }

            //for each row: have or not children
            td.Data.Schema.Add("isFolder");
            foreach (var r in filtred)
            {
                var key = (r[keyCol] ?? "0").GetHashCode();
                int isFolder = 0;
                foreach (var r1 in td.Data.Rows)
                {
                    var group = (r1[groupCol] ?? "0").GetHashCode();
                    if (group == key)
                    {
                        isFolder = 1;
                        break;
                    }
                }
                r.Add(isFolder);
            }

            td.Data.Rows.Clear();
            td.Data.Rows.AddRange(filtred);
        }

        void Sort(TableData td, string orderBy)
        {
            TableContent d = td.Data;
            if (d.Rows.Count == 0)
                return;

            int oCol = -1;
            string[] orderByList = { };
            if (!string.IsNullOrEmpty(orderBy))
            {
                orderBy = orderBy.ToLower();
                orderByList = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < d.Schema.Count; i++)
                {
                    if (d.Schema[i].ToLower() == orderByList[0])
                    {
                        oCol = i;
                        break;
                    }
                }
            }

            if (oCol != -1)
            {
                if (orderByList.Length > 1 && orderByList[1] == "desc")
                    d.Rows.Sort((i1, i2) => -((IComparable)i1[oCol]).CompareTo(i2[oCol]));
                else
                    d.Rows.Sort((i1, i2) => ((IComparable)i1[oCol]).CompareTo(i2[oCol]));
//                    d.Rows.OrderBy(r => r[oCol]);
                td.OrderByColumn = orderBy;
            }
        }

        void ReadDataJson(string content, TableContent d)
        {
            var td = JsonManager.ReadJsonString(content);

            var names = new Dictionary<string, bool>();
            foreach (var r in td)
            {
                foreach (var n in r.Keys)
                {
                    var n1 = n.ToLower();
                    if (names.ContainsKey(n1))
                        continue;
                    names.Add(n1, true);
                }
            }
            d.Schema.AddRange(names.Keys);

            foreach (var r in td)
            {
                var tr = new TableRow();

                var comparer = StringComparer.OrdinalIgnoreCase;
                var r1 = new Dictionary<string, object>(r, comparer);
                foreach (var n in names.Keys)
                {
                    if (r1.ContainsKey(n))
                        tr.Add(r1[n]);
                }

                d.Rows.Add(tr);
            }
        }

        void ReadDataCsv(string content, TableContent d)
        {
            CsvManager.TextTrim = true;
            var td = CsvManager.ReadCsvString(content);
            d.Schema.AddRange(td.Header);
            foreach (var r in td.Content)
            {
                var tr = new TableRow();
                tr.AddRange(r);
                d.Rows.Add(tr);
            }
        }
    }
}
