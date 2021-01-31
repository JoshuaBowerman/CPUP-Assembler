using System;
using System.Collections.Generic;
using System.Text;

namespace CPUPA
{
    class Assembler
    {

        public static Library assemble(bool isLib, string[] input)
        {
            //Important Variables that will be used throughout
            Library lib = new Library(isLib);
            int variableIndex = 0; //What Variable ID is next to be used;
            int functionIndex = 0; //What Function ID is next to be used;
            Dictionary<string, int> variables = new Dictionary<string, int>(); // The int is the variable id
            Dictionary<string, int> functions = new Dictionary<string, int>(); // int is the function id
            List<string> exposedFunctions = new List<string>();


            //Assembling
            //State
            bool definingFunction = false; // whether or not we are in a function block.
            string functionName = ""; //The name of the function we are defining
            int functionStartIndex = 0; //This is the address that the function begins at in the library code list.

            for(int i = 0; i < input.Length; i++)
            {
                string line = input[i];
                if (!definingFunction) //Not Defining
                {
                    if (line.StartsWith("data")){
                        //check that variable name is good
                        if(!line.Split(" ")[1].StartsWith("."))
                        {
                            Console.WriteLine("Invalid Variable Name {0} line:\n{1}", line.Split(" ")[1], line);
                            throw new Exception("-3");
                        }
                    }
                }
                else //Defining a function
                {

                }
            }



            return lib;
        }

    }
}
