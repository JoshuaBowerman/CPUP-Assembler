using System;
using System.Collections.Generic;
using System.Text;
using CPUPA;
using System.Linq;

namespace CPUPLinker
{
    class Linker
    {
        private List<ushort> machineCode;
        private List<Library> libraries;
        private bool isVerbose = false;
        public Linker(bool verbose)
        {
            machineCode = new List<ushort>();
            libraries = new List<Library>();
            isVerbose = verbose;
        }

        public void addLibrary(Library lib)
        {
            libraries.Add(lib);
        }


        /* Links all the provided libraries into
         * a set of machine code.
         * Does not return anything
         */
        public void Link()
        {

            //Find the main program library
            Library mainLib = null;
            for(int i = 0; i < libraries.Count; i++)
            {
                if (!libraries[i].isLibrary)
                {
                    //Check to make sure we haven't already found one
                    if(mainLib != null)
                    {
                        Console.WriteLine("More Than One Program File Found(Files containing a main function).\nFound {0}, Already had {1}", libraries[i].fileName, mainLib.fileName);
                        throw new Exception("-3");
                    }
                    mainLib = libraries[i];
                    libraries.RemoveAt(i);
                    i--;
                }
            }

            //Now we have mainLib which is our entry point and librairies which is all out libraries

            //We need to make  one master list of addresses, variables, and function IDs translated into new addresses
            //This will be done as the libraries are transfered into the new structure

            //First int is the ID second ushort is location index in the new structure. This contains all internal IDs
            Dictionary<int, ushort> masterAddressTable = new Dictionary<int, ushort>();
            //string is name, int is new Master ID These are for provided functions, not the ones imported.
            Dictionary<string, int> masterProvidedTable = new Dictionary<string, int>();
            //string is name, int is a new Master ID. These are for functions that have been imported
            Dictionary<int, string> masterImportedTable = new Dictionary<int, string>();

            //This is the next used ID
            int IDIndex = 1; // Don't use 0 as an id

            //Near the end the provided and imported tables will be used to link the uses of a function with their declaration

            //This will be the working machine code table. it uses int instead of ushort because it still contains IDs
            List<int> workingMachineCode = new List<int>();

            Console.WriteLine("Begining Linking");

            //Start with the main library
            //To do this i'm just going to insert the mainLib into the begining of the libraries list
            libraries.Insert(0, mainLib);

            foreach(Library lib in libraries)
            {
                Console.WriteLine("   {0}", lib.fileName);

                //This libraries offset from the start of the binary
                ushort offset = unchecked((ushort)workingMachineCode.Count);


                //Generate Translation Tables Old ID, New ID
                //And Generate Correct Addresses to insert into the masterAddressTable
                Dictionary<int, int> translationTable = new Dictionary<int, int>();

                //Internal Addresses
                foreach(var entry in lib.internalAddresses)
                {
                    if(isVerbose)
                        Console.WriteLine("INFO: Internal Address,  Index: {0,3} Key: {1,3} Value: {2,5}", IDIndex, entry.Key,entry.Value);
                    //Translation Entry
                    translationTable.Add(entry.Key, -IDIndex);

                    // Master Address Table entry
                    masterAddressTable.Add(-IDIndex, (ushort)(entry.Value + offset));

                    //Increment IDIndex
                    IDIndex++;
                }
                //Internal Variables
                foreach (var entry in lib.internalVariables)
                {
                    if (isVerbose)
                        Console.WriteLine("INFO: Internal Variable, Index: {0,3} Key: {1,3} Value: {2,5}", IDIndex, entry.Key, entry.Value);
                    //Translation Entry
                    translationTable.Add(entry.Key, -IDIndex);

                    // Master Address Table entry
                    masterAddressTable.Add(-IDIndex, (ushort)(entry.Value + offset));

                    //Increment IDIndex
                    IDIndex++;
                }
                //Provided Functions
                foreach (var entry in lib.providedFunctions)
                {
                    //Provided Function Table Entry
                    masterProvidedTable.Add(entry.Key, translationTable[entry.Value]);
                }
                //Imported Functions
                foreach (var entry in lib.importedFunctions)
                {
                    //Imported Function Table Entry
                    masterImportedTable.Add( translationTable[entry.Value], entry.Key);
                }

                //Transfer and translate library
                for(int i = 0; i < lib.code.Count; i++)
                {
                    //IDs
                    if(lib.code[i] < 0)
                    {
                        workingMachineCode.Add(translationTable[lib.code[i]]);
                    }
                    else
                    {
                        workingMachineCode.Add(lib.code[i]);
                    }

                }
            }
            Console.WriteLine("  Translating IDs");
            //Translate IDs to Addresses
            for(int i = 0; i < workingMachineCode.Count; i++)
            {
                if(workingMachineCode[i] < 0)
                {
                    int temp = workingMachineCode[i];
                    //an ID

                    //Check to see if it's in the master address table
                    if (masterAddressTable.ContainsKey(workingMachineCode[i]))
                    {
                        workingMachineCode[i] = masterAddressTable[workingMachineCode[i]];
                    }
                    else
                    {
                        //It's an imported function.

                        //Find the function key

                        string funcName = masterImportedTable[workingMachineCode[i]];

                        //check to see if it was provided
                        if (!masterProvidedTable.ContainsKey(funcName))
                        {
                            Console.WriteLine("ERROR: No function provided for name : {0}", funcName);
                            throw new Exception("-4");
                        }

                        int funcID = masterProvidedTable[funcName];
                        ushort newAddr = masterAddressTable[funcID];

                        workingMachineCode[i] = newAddr;
                    }
                    if (isVerbose)
                        Console.WriteLine("INFO: Translating, ID: {0,3} Location: {1,5} Translation: {2,5}", temp, i, workingMachineCode[i]);
                }
            }

            //We are done linking

            Console.WriteLine("Done Linking");

            //Transfer Integer Array to Ushort machinecode
            for(int i = 0; i < workingMachineCode.Count; i++)
            {
                machineCode.Add(unchecked((ushort)workingMachineCode[i]));
            }
        }

        public ushort[] getMachineCode()
        {
            return machineCode.ToArray();
        }

    }
}
