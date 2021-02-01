using System;
using System.Collections.Generic;
using System.Text;

namespace CPUPA
{
    /* This class is the library created by the assembler.
     * It is built during the assembly process, and is eventually serialized as XML to become the LIB file.
     * 
     */
    public class Library
    {
        // this is the list of functions exposed by this library.
        // string is the function name, int is the index of the first instruction
        public Dictionary<string, int> providedFunctions;

        // this is the list of used functions from other libraries.
        // the string being the desired function's name and the int being the ID used.
        public Dictionary<string, int> importedFunctions;

        //these are the instructions that make up the program positive values are regular, negative values are memory addresses
        public List<Int32> code;

        //These are the variables(memory locations) used by the program. first int being the value used in the code above, second being the size in words.
        //The linker only needs to worry about attached memory locations.
        public Dictionary<int, int> internalVariables;

        //All instruction addresses used by the programmer are translated into an ID which is the first int. 
        // The second int is the location of the desired address as the index of the instruction in the code list.
        public Dictionary<int, int> internalAddresses;

        //Whether or not this was compiled as a library or a regular program.
        //non library programs are expected to have the main function at address 0
        public bool isLibrary;

        public Library(bool isLib)
        {
            isLibrary = isLib;
            providedFunctions = new Dictionary<string, int>();
            code = new List<Int32>();
            internalVariables = new Dictionary<int, int>();
            internalAddresses = new Dictionary<int, int>();
            importedFunctions = new Dictionary<string, int>();

        }

        public Library()
        {
            isLibrary = false;
            providedFunctions = new Dictionary<string, int>();
            code = new List<Int32>();
            internalVariables = new Dictionary<int, int>();
            internalAddresses = new Dictionary<int, int>();
            importedFunctions = new Dictionary<string, int>();
        }
    }
}
