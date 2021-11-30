_Please refer to 'How to run' section if get DirectoryNotFoundException_

# TreeGrid demo project
This is demo project on ASP.NET Core for [treegrid](https://github.com/miptleha/treegrid-js) component.   
Install Visual Studio to run project ([free](https://visualstudio.microsoft.com/en/vs/community/) version).   

Now for Visual Studio 2019 and .NET 5.   
New version is without gulp file.   
Newtonsoft.Json package is replaced with System.Text.Json.

## Download source code
```
git clone https://github.com/miptleha/treegrid-core.git
```

## Where is frontent part?

This project contains only .NET backend part and [View](Views/Home/Index.cshtml) with initialization [code](wwwroot/js/site.js) for treegrid JavaScript component.    
Treegrid component is installed as NPM module (dependencies in [package.json](package.json)) from GitHub [repository](https://github.com/miptleha/treegrid-js).

## How to run
* Open the treegrid-core.sln file in Visual Studio
* Right click on package.json and select 'Restore Packages'
* Frontent component and jquery will be uploaded in node_modules folder
* Run application
