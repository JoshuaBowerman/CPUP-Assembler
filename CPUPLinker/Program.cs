using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace CPUPLinker
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"CPUP Linker
==============
This is the CPUP Linker, it accepts files produced by the Assembler, it combines them into an output format of your choice.
It accepts just one or multiple .LIB files, it will link these into a single program. Only one of the input files can be a Program file as this will be used as the entry point.

It is recommended that you use CPUP-Build to build your program rather than calling the linker yourself. 


Format:
 CPUPA.exe [Switches] Input1 Input2 Input3...
 
Defaults:
 FileType: MIF
 Output File: ./out.mif

Switches:
 -of=VAL    | Sets the output file. do not use a space between the = and the file name. if your file name has spaces enclose it inside double quotes.
 -hex       | Outputs a .hex file (Intel Hexadecimal File)
 -bin       | Outputs a .bin file (Binary File)
 -mif       | Outputs a .mif file (Intel Memory Initialization File)
 
");
                return 1;
            }

            //Program Settings
            string outputType = "mif"; //this will be set to the desired output type, defaulting to mif
            string outputFile = "out.mif";

            List<string> inputFileNames = new List<string>();

            foreach(string arg in args)
            {
               //Simple Switches
               if(arg.StartsWith("-") && !arg.Contains("="))
                {
                    switch (arg)
                    {
                        case "-hex":
                            outputType = "hex";
                            break;
                        case "-bin":
                            outputType = "bin";
                            break;
                        case "-mif":
                            outputType = "mif";
                            break;
                        default:
                            Console.WriteLine("ERROR: Unkown Switch: {0}", arg);
                            return -1;
                    }
                }
               //OF Switch
               else if (arg.StartsWith("-of="))
                {
                    outputFile = arg.Substring(4);
                }
               //Input File Name
                else
                {
                    inputFileNames.Add(arg);
                }
            }

            //Check to make sure each file exists
            foreach(string fileName in inputFileNames)
            {
                if (!File.Exists(fileName))
                {
                    Console.WriteLine("ERROR: Input file does not exist: {0}", fileName);
                    return -2;
                }
            }

            //Turn each input into a libray object
            Linker linker = new Linker();
            foreach(string fileName in inputFileNames)
            {
                Console.WriteLine("Importing {0}", fileName);
                FileStream fs;
                XmlDictionaryReader reader;
                DataContractSerializer ser;
                CPUPA.Library lib;
                try
                {
                    fs = new FileStream(fileName, FileMode.Open);
                    reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                    ser = new DataContractSerializer(typeof(CPUPA.Library));
                    lib = (CPUPA.Library)ser.ReadObject(reader, true);
                    reader.Close();
                    fs.Close();
                }
                catch( Exception e)
                {
                    Console.WriteLine("ERROR: Could not read input file {0}\n Error: {1}", fileName, e.Message);
                    return -5;
                }

                linker.addLibrary(lib);
            }

            //Link
            linker.Link();

            //Output formatting
            byte[] output;
            Format.IFormat formatter;
            switch (outputType)
            {
                case "mif":
                    formatter = new Format.MIF();
                    break;
                case "hex":
                    formatter = new Format.HEX();
                    break;
                case "bin":
                    formatter = new Format.BIN();
                    break;
                default:
                    Console.WriteLine("ERROR: Invalid Format: {0}", outputType);
                    return -1;
            }
            output = formatter.getOutput(linker.getMachineCode());

            try
            {
                File.WriteAllBytes(outputFile, output);
            }catch( Exception e)
            {
                Console.WriteLine("ERROR: Could Not Write Output File {0}\n Error: {1}", outputFile, e.Message);
                return -6;
            }
            return 0;
        }
    }
}
