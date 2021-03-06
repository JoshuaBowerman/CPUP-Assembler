﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace CPUPA
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(@"CPUP Assembler
==============
This is the CPUP Assembler, it accepts files written in CPUP Assembly, it outputs LIB files, These can be used in the linker.

Format:
CPUPA.exe [Switches] inputFile.cpa output.lib

Switches:
 -lib   | Does not have a main function, process as a library.
 -ss=VAL| Overide the stack size with VAL, default stack size is 4096 words
 -v     | Verbose Mode, Prints addresses, IDs etc.
 
");
                return 0;
            }

            //Deal with arguments

            List<string> switches = new List<string>();
            string inputLocation = "";
            string outputLocation = "";

            foreach(string arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    switches.Add(arg);
                }
                else
                {
                    if(inputLocation == "")
                    {
                        inputLocation = arg;
                    }
                    else
                    {
                        outputLocation = arg;
                    }
                }
            }
            bool isLib = false;
            bool verbose = false;
            int stackSize = 4096;
            foreach(string sw in switches){
                switch (sw.Split("=")[0])
                {
                    case "-lib":
                        isLib = true;
                        break;
                    case "-ss":
                        try
                        {
                            stackSize = int.Parse(sw.Split("=")[1]);
                        }catch(Exception e)
                        {
                            Console.WriteLine("Could Not Parse Command Switch -SS, Unable to parse value {0} Error:\n{1}",sw, e.Message);
                            return -2;
                        }
                        break;
                    case "-v":
                        verbose = true;
                        break;
                    default: Console.WriteLine("Unknown Switch: {0}", sw);
                        return -2;
                }
            }

            //Read Input File
            string[] input;
            try
            {
                input = File.ReadAllLines(inputLocation);
            }catch(Exception e)
            {
                Console.WriteLine("Could Not Read Input File:{0}", inputLocation);
                Console.WriteLine(e.Message);
                return -1;
            }
            Console.WriteLine("Formatting");
            //Lint File
            //It's not actually a linter
            //It fixes formatting and replaces translated instructions with their actual code
            string[] assembly;
            try
            {
                assembly = Linter.Lint(input, stackSize, isLib, verbose);
            }
            catch(Exception e)
            {
                return int.Parse(e.Message);
            }


            Console.WriteLine("Assembling");


            //Assemble
            Library output;
            try
            {
                output = Assembler.assemble(isLib, assembly, verbose);
            }
            catch(Exception e)
            {
                return int.Parse(e.Message);
            }

            output.fileName = outputLocation;

            //Write

            FileStream writer = new FileStream(outputLocation, FileMode.Create);
            DataContractSerializer ser;
            try
            {
                ser = new DataContractSerializer(typeof(Library));
                ser.WriteObject(writer, output);
                writer.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Could not write output file: {0}", outputLocation);
                Console.WriteLine(e.Message);
            }




            return 0;
        }
    }
}
