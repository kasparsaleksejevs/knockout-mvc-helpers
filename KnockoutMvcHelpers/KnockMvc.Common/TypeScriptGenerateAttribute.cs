using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace KnockMvc.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class TypeScriptGenerateAttribute : Attribute
    {
        public static string GenerateJsFiles(Assembly sourceAssembly, IServiceProvider host)
        {
            var types = new List<Type>();

            foreach (var c in sourceAssembly.GetExportedTypes())
            {
                if (c.GetCustomAttribute<TypeScriptGenerateAttribute>(false) == null)
                    continue;

                types.Add(c);
            }

            var files = new List<Tuple<string, string>>();
            foreach (var item in types.GroupBy(m => m.Namespace))
            {
                var sb = new StringBuilder();
                var ns = item.Key;
                sb.AppendLine("module " + ns + " {");
                foreach (var ty in item)
                {
                    sb.AppendLine("\tclass " + ty.Name + " {");

                    foreach (var member in ty.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var prop = member as PropertyInfo;
                        if (prop == null)
                            continue;

                        var propertyType = GetPropertyTypeDefinition(prop);

                        sb.AppendLine("\t\t" + prop.Name + " = " + propertyType);
                    }

                    sb.AppendLine("\t}");
                }

                sb.AppendLine("}");

                files.Add(new Tuple<string, string>(item.Key, sb.ToString()));
            }

            new TypeScriptFileAdder().SaveCustomFiles(files, host);

            return string.Join("\r\n", files.Select(s => s.Item2));
        }

        private static string GetPropertyTypeDefinition(PropertyInfo property)
        {
            var nullableSymbol = string.Empty; //property.IsNullable ? "?" : string.Empty;
            var propertyType = "string";

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?)
                || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?)
                || property.PropertyType == typeof(double) || property.PropertyType == typeof(double?)
                || property.PropertyType == typeof(float) || property.PropertyType == typeof(float?)
                )
                propertyType = "number";

            if (property.PropertyType == typeof(string))
                propertyType = "string";

            if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                propertyType = "boolean";

            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                propertyType = "Date";

            // ToDo: enums
            // ToDo: arrays
            // ToDo: custom classes

            return $"ko.observable<{propertyType}{nullableSymbol}>();";
        }
    }

    internal class TypeScriptFileAdder
    {
        public void SaveCustomFiles(List<Tuple<string, string>> files, IServiceProvider serviceProvider)
        {
            DTE dte = (DTE)serviceProvider.GetService(typeof(DTE));

            // ToDo: do we need compile TS at all? IMHO JS generated from the TS shouldn't be needed at all...
            //var tsCompilerPath = this.GetTypeScriptCompilerLocation();

            // ToDo: the project name should be found from the t4 file location
            var projectName = "KnockMvc.Web";
            var webProject = dte.Solution.Projects.Cast<Project>().Single(p => p.Name == projectName);

            var projectFileName = webProject.FileName;
            var path = Path.Combine(Path.GetDirectoryName(projectFileName), "Scripts", "viewmodels");
            foreach (var item in files)
            {
                var tsFileName = Path.Combine(path, $"model_{item.Item1}.ts");

                // ToDo: this check should be refactored to use DTE to find actual file status - is it in the project or not
                var fileExistsInProject = File.Exists(tsFileName);
                File.WriteAllText(tsFileName, item.Item2);

                //var jsFileName = CompileTs(tsCompilerPath, tsFileName);

                if (!fileExistsInProject)
                {
                    webProject.ProjectItems.AddFromFile(tsFileName);
                    //webProject.ProjectItems.AddFromFile(jsFileName);
                }

                webProject.Save();
            }
        }

        private string CompileTs(string tsCompilerPath, string fileName)
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = tsCompilerPath,
                    Arguments = $"--target ES5 \"{fileName}\""
                }
            };
            process.Start();
            process.WaitForExit();

            return Path.ChangeExtension(fileName, ".js");
        }

        private string GetTypeScriptCompilerLocation()
        {
            // run tsc.exe from here C:\Program Files (x86)\Microsoft SDKs\TypeScript\1.8\tsc.exe
            // but first read the csproj file to get "<TypeScriptToolsVersion>1.8</TypeScriptToolsVersion>"
            // if tsc doesn't exist in programFiles, search solution packages: packages\Microsoft.TypeScript.Compiler.2.6.2\tools\tsc.exe

            // trying to detect the 'tsc.exe' location...

            var regex = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}");

            // we could find one in C:\Program Files (x86)\Microsoft SDKs\TypeScript\2.6\tsc.exe
            var tsFolders = new List<TypeScriptCompilerData>();
            var typeScriptProgFilesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft SDKs", "TypeScript");
            if (Directory.Exists(typeScriptProgFilesFolder))
            {
                foreach (var tsDirectory in Directory.GetDirectories(typeScriptProgFilesFolder))
                {
                    var fullPath = Path.Combine(tsDirectory, "tsc.exe");
                    var version = Path.GetFileName(Path.GetDirectoryName(fullPath));

                    if (File.Exists(fullPath) && regex.IsMatch(version))
                    {
                        tsFolders.Add(new TypeScriptCompilerData { Version = version, LocationPath = fullPath });
                    }
                }
            }

            // we could find one in the solutions' packages, e.g. 'Microsoft.TypeScript.Compiler.2.7.2'
            // ToDo: search the installed NuGet packages (first - the Web project packages.json, then - the solutions' packages folder)

            return tsFolders.OrderByDescending(m => m.Version).FirstOrDefault()?.LocationPath;
        }

        private class TypeScriptCompilerData
        {
            public string Version { get; set; }

            public string LocationPath { get; set; }
        }
    }
}
