using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TreeGridCore.Code.Table;
using TreeGridCore.Models;

namespace TreeGridCore.Controllers
{
    public class HomeController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        public HomeController(IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Data(string id, string orderBy, string parentId)
        {
            var tm = new TableManager();
            if (string.IsNullOrWhiteSpace(id))
                throw new Exception("Query parameter 'id' not specified for request");

            var appData = Path.Combine(_appEnvironment.ContentRootPath, "App_Data");
            var data = tm.LoadTable(id, orderBy, parentId, appData);

            var serializerSettings = new JsonSerializerOptions 
            {
                PropertyNamingPolicy = null
            };
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
        }
    }
}
