using System;
using System.Diagnostics;
using System.IO;

namespace CPUPuild
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("CPUP Build");
            if(args.Length < 1)
            {
                Console.WriteLine("Usage: CPUPBuild.exe <Project Folder>\nWill Compile and Link all cpa files in project.");

                return 0;
            }

            string linker = @"C:\Users\Josh\source\repos\CPUP Assembler\CPUPLinker\bin\Debug\netcoreapp2.1\win10-x64\CPUPLinker.exe";
            string assembler = @"C:\Users\Josh\source\repos\CPUP Assembler\CPUPA\bin\Debug\netcoreapp2.1\win10-x64\CPUPA.exe";
            string projectDir = args[0];
            if (!projectDir.EndsWith("\\"))
            {
                projectDir += "\\";
            }
            FileInfo[] files;
            try
            {
                DirectoryInfo d = new DirectoryInfo(args[0]);
                files = d.GetFiles("*.cpa");
            }
            catch (Exception e)
            {
                Console.WriteLine("Could Not Open Project: {0}", e.Message);
                return -1;
            }

            //make build dir

            if(!Directory.Exists(projectDir + "build\\"))
            {
                Directory.CreateDirectory(projectDir + "build\\");
                Directory.CreateDirectory(projectDir + "build\\libs");
            }

            foreach (var f in Directory.GetFiles(projectDir + "build\\"))
            {
                File.Delete(f);
            }
            foreach (var f in Directory.GetFiles(projectDir + "build\\libs"))
            {
                File.Delete(f);
            }

            //Create Libs

            foreach (var file in files)
            {
                string[] lines = File.ReadAllLines(file.FullName);
                bool hasMain = false;
                foreach(var line in lines)
                {
                    if(line.ToLower().Trim().Contains("define main"))
                    {
                        hasMain = true;
                    }
                }
                string assemblerArgs = "";
                if (!hasMain)
                {
                    assemblerArgs += "-lib";
                }
                assemblerArgs += " \"" + file.FullName + "\" \"" + projectDir + "build\\libs\\" + file.Name + ".lib\"";

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = assembler;
                p.StartInfo.Arguments = assemblerArgs;
                Console.WriteLine("Assembling File: {0}", file.Name);
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                Console.WriteLine(output);
            }
            DirectoryInfo d = new DirectoryInfo(projectDir + @"build\libs\");
            files = d.GetFiles("*.lib");

            string linkArgs = "-of=\""+projectDir + @"\build\program.mif" + "\"";
            foreach(var file in files)
            {
                linkArgs += " \"" + file.FullName + "\""; 
            }
            Console.WriteLine("Linking Libraries");
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = linker;
            p.StartInfo.Arguments = linkArgs;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            Console.WriteLine(output);
            return 0;
        }
    }
}
