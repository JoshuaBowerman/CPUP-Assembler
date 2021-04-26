using System;
using System.Collections.Generic;
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
                Console.WriteLine(@"CPUP Build Tool
===============
Builds a program out of the files in a folder. looks through each file for a main function and uses that as the program file and compiles the rest as libraries.

Usage: CPUPBuild.exe <Options> <Project Folder>

    Options
===============

-v      | Verbose Mode
-ss=##  | Sets the stack size, eg: -ss=4096

");

                return 0;
            }

            string linker = @"C:\Users\Josh\Documents\GitHub\CPUP-BuildTools\CPUPLinker\bin\Debug\netcoreapp2.1\win10-x64\CPUPLinker.exe";
            string assembler = @"C:\Users\Josh\Documents\GitHub\CPUP-BuildTools\CPUPA\bin\Debug\netcoreapp2.1\win10-x64\CPUPA.exe";
            string projectDir = "";
            List<String> switches = new List<String>();

            //Find the project dir and seperate the switches
            for(int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-"))
                {
                    if(projectDir != "")
                    {
                        //too many folder arguments.
                        Console.WriteLine("ERROR: More than 1 project directory provided. Got\n1: \"{0}\"\n2: \"{1}\"", projectDir, args[i]);
                        return -15;
                    }
                    projectDir = args[i];
                }
                else
                {
                    switches.Add(args[i]);
                }
            }

            //Options
            bool isVerbose = false;
            int stackSize = 4096;

            //switches
            foreach(string entry in switches)
            {
                switch (entry.Split("=")[0])
                {
                    case "-v":
                        isVerbose = true;
                        break;
                    case "-ss":
                        try
                        {
                            stackSize = int.Parse(entry.Split("=")[1]);
                        }catch(Exception e)
                        {
                            Console.WriteLine("ERROR: Could not parse stack size. At: {0}", entry);
                            return -16;
                        }
                        break;
                    default:
                        Console.WriteLine("ERROR: Unknown Swtich: {0}", entry);
                        return -16;
                }
            }

            if (!projectDir.EndsWith("\\"))
            {
                projectDir += "\\";
            }
            FileInfo[] files;
            try
            {
                DirectoryInfo di = new DirectoryInfo(args[0]);
                files = di.GetFiles("*.cpa");
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
                else
                {
                    assemblerArgs += "-ss=" + stackSize;
                }

                if (isVerbose)
                {
                    assemblerArgs += " -v";
                }

                assemblerArgs += " \"" + file.FullName + "\" \"" + projectDir + "build\\libs\\" + file.Name + ".lib\"";

                Process pr = new Process();
                pr.StartInfo.UseShellExecute = false;
                pr.StartInfo.RedirectStandardOutput = true;
                pr.StartInfo.FileName = assembler;
                pr.StartInfo.Arguments = assemblerArgs;
                Console.WriteLine("Assembling File: {0}", file.Name);
                pr.Start();
                pr.WaitForExit();
                Console.WriteLine(pr.StandardOutput.ReadToEnd());
            }
            DirectoryInfo d = new DirectoryInfo(projectDir + @"build\libs\");
            files = d.GetFiles("*.lib");

            string linkArgs = "-of=\""+projectDir + @"\build\program.mif" + "\"";
            if (isVerbose)
                linkArgs += " -v";
            foreach (var file in files)
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
