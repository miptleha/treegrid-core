# TreeGrid demo project
This is demo project on Visual Studio 2017 (ASP.NET Core) for [treegrid](https://github.com/miptleha/treegrid-js) component

## How to start
This project references javascript project on github. If you don't see grids when run project:
<ul>
  <li>Open solution treegrid-core.sln in Visual Studio</li>
  <li>Wait 20 seconds for initializing modules (Visual Studio download all required files in node_modules folder)</li>
  <li>Start debugging, there will be error, stop debugging (hello Microsoft)</li>
  <li>Start debugging again (Visual Studio will run gulp task, that copies files to wwwroot)</li>
  <li>If grid still not visible manualy run grunt task from context menu on gruntfile.js in Visual Studio</li>
</ul>

Do not do changes in wwwroot folder, after build in Visual Studio your changes disapear (gulp overwrite files from node_modules folder). Modify files in node_modules/treegrid or comment tasks in gruntfile.js.
