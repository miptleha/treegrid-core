using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TreeGridCore.Code.Table;
using TreeGridCore.Models;

namespace TreeGridCore.Controllers
{
    public class HomeController : Controller
    {
        IHostingEnvironment _appEnvironment;
        public HomeController(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Data(string id, string orderBy, string parentId)
        {
            //System.Threading.Thread.Sleep(10000);
            //var path = _appEnvironment.WebRootPath;
            //path = Path.Combine(path, "App_Data");
            //path = Path.Combine(path, "employees.csv");
            //var td = CsvManager.ReadCsv(path);
            //var data = new ReportData();
            //data.Columns = new List<ReportColumn>();
            //var c = new ReportColumn { Caption = "Employee info" };
            //data.Columns.Add(c);
            //foreach (var h in td.Header)
            //    c.Children.Add(new ReportColumn { Caption = h, Name = h, Width = 100 });
            //data.Result = new DbTable();
            //foreach (var r in td.Content)
            //{
            //    var dr = new DbTableDataRow(r.Count);
            //    for (int i = 0; i < r.Count; i++)
            //        dr[i] = r[i];
            //    data.Result.Data.Add(dr);
            //}

            //var serializerSettings = new JsonSerializerSettings();
            //serializerSettings.ContractResolver = new DefaultContractResolver();

            var tm = new TableManager();
            if (string.IsNullOrWhiteSpace(id))
                throw new Exception("Query parameter 'id' not specified for request");

            var appData = Path.Combine(_appEnvironment.ContentRootPath, "App_Data");
            var data = tm.LoadTable(id, orderBy, parentId, appData);

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new DefaultContractResolver();

            return Json(data, serializerSettings);
        }

        public IActionResult Xml(string id)
        {
            var tm = new TableManager();
            if (string.IsNullOrWhiteSpace(id))
                throw new Exception("Query parameter 'id' not specified for request");

            var appData = Path.Combine(_appEnvironment.ContentRootPath, "App_Data");
            var path = tm.GetXmlPath(id, appData);

            return View(new FileXml { Path = path });

            return PhysicalFile(path, "text/html", Path.GetFileNameWithoutExtension(path) + ".html");
        }
    }
}
