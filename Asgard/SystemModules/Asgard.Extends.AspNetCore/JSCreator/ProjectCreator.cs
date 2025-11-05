using System.Reflection;
using System.Text;

namespace Asgard.Extends.AspNetCore.JSCreator
{
    internal class ProjectCreator
    {
        private string SrcFolder { get; init; }
        private string SourceCodeFolder { get; init; }
        private string Folder { get; init; }
        private Dictionary<string, TempCodeInfo> AllControllerCode { get; init; }
        private string Version { get; init; }
        private Assembly Ass { get; init; }

        private Dictionary<string, List<string>> AllApis { get; init; }

        internal ProjectCreator(string folder, Dictionary<string, TempCodeInfo> allControllerCode, Assembly ass, Dictionary<string, List<string>> apis)
        {
            AllControllerCode = allControllerCode;
            Folder = folder;
            SourceCodeFolder = Path.Combine(Folder, ass.GetName().Name!);
            Ass = ass;
            AllApis = apis;
            SrcFolder = Path.Combine(SourceCodeFolder, "src");
            _ = Directory.CreateDirectory(SourceCodeFolder);
            Version = ass!.GetName().Version!.ToString();
            Version = Version.Substring(0, Version.LastIndexOf("."));
            foreach (var item in Directory.GetFileSystemEntries(SourceCodeFolder))
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
                else
                {
                    Directory.Delete(item, true);
                }
            }
            _ = Directory.CreateDirectory(SrcFolder);

        }


        internal string Create()
        {
            CreatePackageJson();
            CreateReadMe();
            CreateTSConfig();
            CreateIndexTs();
            CreateSourceCode();
            {
                var sh = File.CreateText(Path.Combine(SourceCodeFolder, "unPublish.ps1"));
                sh.WriteLine($"npm unpublish  {Ass.GetName().Name!.Replace(".", "-").ToLower()}@{Version} -f");
                sh.WriteLine($"Start-Sleep -Seconds 150");
                sh.Close();
                sh.Dispose();
            }
            {
                var sh = File.CreateText(Path.Combine(SourceCodeFolder, "publish.ps1"));
                sh.WriteLine($"npm install");
                sh.WriteLine($"npm install asgard-fe-core -D");
                sh.WriteLine($"Start-Sleep -Seconds 2");
                sh.WriteLine($"tsc");
                sh.WriteLine($"Start-Sleep -Seconds 2");
                sh.WriteLine($"npm publish");
                sh.WriteLine($"Start-Sleep -Seconds 150");
                sh.Close();
                sh.Dispose();
            }
            return SourceCodeFolder;


        }


        private void CreateIndexTs()
        {
            using var ft = File.CreateText(Path.Combine(SrcFolder, "index.ts"));
            var codeBuilder = new StringBuilder();
            foreach (var item in AllControllerCode.Keys)
            {
                _ = codeBuilder.AppendLine($"import {{ {item}Module }} from \"./{item}\"");
                _ = codeBuilder.AppendLine();
            }

            _ = codeBuilder.AppendLine($"export namespace {Ass.GetName().Name}{{");

            foreach (var item in AllControllerCode.Keys)
            {
                _ = codeBuilder.AppendLine($"\t export import {item}={item}Module;");
            }

            _ = codeBuilder.AppendLine("}");


            ft.WriteLine(codeBuilder.ToString());
        }

        private void CreateSourceCode()
        {
            foreach (var item in AllControllerCode.Keys)
            {
                using var controller = File.CreateText(Path.Combine(SrcFolder, $"{item}.ts"));
                controller.Write(AllControllerCode[item].AllCode);
            }
        }

        private void CreateTSConfig()
        {

            using var ft = File.CreateText(Path.Combine(SourceCodeFolder, "tsconfig.json"));
            ft.WriteLine("           {");
            ft.WriteLine(" \"compilerOptions\": {");
            ft.WriteLine("      \"target\": \"ES6\",");
            ft.WriteLine("      \"module\": \"ES6\",");
            ft.WriteLine("      \"declaration\": true,");
            ft.WriteLine("      \"outDir\": \"./dist\",");
            ft.WriteLine("      \"esModuleInterop\": true,");
            ft.WriteLine("      \"forceConsistentCasingInFileNames\": true,");
            ft.WriteLine("      \"strict\": true,");
            ft.WriteLine("      \"skipLibCheck\": true,");
            ft.WriteLine("      \"moduleResolution\": \"node\"");
            ft.WriteLine("    },");
            ft.WriteLine(" \"include\": [");
            ft.WriteLine("   \"./src\"");
            ft.WriteLine(" ],");
            ft.WriteLine(" \"exclude\": [");
            ft.WriteLine("   \"./node_modules/*\"");
            ft.WriteLine(" ]");
            ft.WriteLine("}");
        }
        private void CreateReadMe()
        {
            using var ft = File.CreateText(Path.Combine(SourceCodeFolder, "readme.md"));
            AllApis.Keys.ToList().ForEach(item =>
            {
                ft.WriteLine($"# {item}");
                AllApis[item].ForEach(apiInfo =>
                {
                    ft.WriteLine($"{apiInfo}");
                });
            });
        }
        private void CreatePackageJson()
        {
            using var ft = File.CreateText(Path.Combine(SourceCodeFolder, "package.json"));
            var assInfo = Ass.GetName();
            ft.WriteLine(@"{");
            ft.WriteLine($"\"name\": \"{assInfo.Name!.Replace(".", "-").ToLower()}\",");
            ft.WriteLine($"  \"version\": \"{Version}\",");
            ft.WriteLine($"  \"description\": \"{assInfo.Name} api resource.\",");
            ft.WriteLine("  \"main\": \"dist/index.js\",");
            ft.WriteLine("  \"types\": \"dist/index.d.ts\",");
            ft.WriteLine("  \"build\": \"tsc\",");
            ft.WriteLine("  \"scripts\": {},");
            ft.WriteLine("  \"keywords\": [],");
            ft.WriteLine("  \"author\": \"Odin\",");
            ft.WriteLine("  \"license\": \"ISC\",");
            ft.WriteLine("  \"release\": \"tsc && npm publish\",");
            ft.WriteLine("  \"devDependencies\": {},");
            ft.WriteLine("  \"dependencies\": {}");
            ft.WriteLine("}");

        }

    }
}
