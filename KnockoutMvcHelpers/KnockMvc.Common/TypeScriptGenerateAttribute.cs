using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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

                        sb.AppendLine("\t\t" + prop.Name + " = ko.observable<number>();");
                    }

                    sb.AppendLine("\t}");
                }

                sb.AppendLine("}");

                files.Add(new Tuple<string, string>(item.Key, sb.ToString()));
            }

            SaveCustomFiles(files, host);

            return string.Join("\r\n", files.Select(s => s.Item2));
        }

        private static void SaveCustomFiles(List<Tuple<string, string>> files, IServiceProvider serviceProvider)
        {
            DTE dte = (DTE)serviceProvider.GetService(typeof(DTE));

            var projectName = "KnockMvc.Web";
            var webProject = dte.Solution.Projects.Cast<Project>().Single(p => p.Name == projectName);

            var projectFileName = webProject.FileName;
            var path = Path.Combine(Path.GetDirectoryName(projectFileName), "Scripts", "viewmodels");
            foreach (var item in files)
            {
                var tsFileName = Path.Combine(path, $"model_{item.Item1}.ts");

                var fileExists = File.Exists(tsFileName);
                File.WriteAllText(tsFileName, item.Item2);

                var jsFileName = CompileTs(tsFileName);

                if (!fileExists)
                {
                    webProject.ProjectItems.AddFromFile(tsFileName);
                    webProject.ProjectItems.AddFromFile(jsFileName);
                }

                webProject.Save();
            }
        }

        private static string CompileTs(string fileName)
        {
            // run tsc.exe from here C:\Program Files (x86)\Microsoft SDKs\TypeScript\1.8\tsc.exe
            // but first read the csproj file to get "<TypeScriptToolsVersion>1.8</TypeScriptToolsVersion>"
            // if tsc doesn't exist in programFiles, search solution packages: packages\Microsoft.TypeScript.Compiler.2.6.2\tools\tsc.exe

            var process = new System.Diagnostics.Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = /*fullPath+*/"tsc.exe",
                    Arguments = $"--target ES5 \"{fileName}\""
                }
            };
            process.Start();
            process.WaitForExit();

            return Path.ChangeExtension(fileName, ".js");
        }

    }
}
