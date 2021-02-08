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
            int ObjectIndex = 1; //This is used to keep track of which IDs have been used. Using one counter ensures that function id's and variable id's do not collide. This makes it easier later.
            Dictionary<string, int> variables = new Dictionary<string, int>(); // The int is the variable id
            Dictionary<string, int> functions = new Dictionary<string, int>(); // int is the function id
            Dictionary<string, int> pointers = new Dictionary<string, int>(); // int is the pointer id
            Dictionary<string,int> exposedFunctions = new Dictionary<string, int>();
            Dictionary<string,int> importedFunctions = new Dictionary<string, int>();


            //Step one is generating internal ID's for each variable and function.
            //This allows programs to use data and functions that are defined later on in the file than itself.
            Console.WriteLine("    Generating Internal ID's");
            for(int i = 0; i < input.Length; i++)
            {
                //Registering Data segments
                if (input[i].StartsWith("data"))
                {
                    //check that variable name is good
                    if (!input[i].Split(" ")[1].StartsWith("."))
                    {
                        Console.WriteLine("Invalid Variable Name {0} line:\n{1}", input[i].Split(" ")[1], input[i]);
                        throw new Exception("-3");
                    }
                    //Register the variable ID
                    variables.Add(input[i].Split(" ")[1], - ObjectIndex++);
                }
                //Registering Functions
                if (input[i].StartsWith("define"))
                {
                    //make sure that there is no spaces and that there actually is a name.
                    if(input[i].Split(' ').Length > 2 || input[i].Split(' ').Length == 1)
                    {
                        Console.WriteLine("Invalid Function Declaration: Name is missing or invalid. line:\n{0}", input[i]);
                        throw new Exception("-3");
                    }
                    //Register the function ID only if this function hasn't been registered yet.
                    if(!functions.ContainsKey(input[i].Split(" ")[1]))
                    {
                        functions.Add(input[i].Split(" ")[1], -ObjectIndex++);
                    }
                }
                //Registering External Functions
                if (input[i].StartsWith("using"))
                {
                    //make sure that there is no spaces and that there actually is a name.
                    if (input[i].Split(' ').Length > 2 || input[i].Split(' ').Length == 1)
                    {
                        Console.WriteLine("Invalid External Function Declaration: Name is missing or invalid. line:\n{0}", input[i]);
                        throw new Exception("-3");
                    }
                    //Register the external function
                    //This will be deduplicated later
                    functions.Add(input[i].Split(" ")[1], - ObjectIndex++);
                    importedFunctions.Add(input[i].Split(" ")[1], - (ObjectIndex - 1));
                }
                //Register Exposed Function
                if (input[i].StartsWith("expose"))
                {
                    //make sure that there is no spaces and that there actually is a name.
                    if (input[i].Split(' ').Length > 2 || input[i].Split(' ').Length == 1)
                    {
                        Console.WriteLine("Invalid External Function Declaration: Name is missing or invalid. line:\n{0}", input[i]);
                        throw new Exception("-3");
                    }
                    //Register the exposed function
                    functions.Add(input[i].Split(" ")[1], -ObjectIndex++);
                    lib.providedFunctions.Add(input[i].Split(" ")[1], -(ObjectIndex - 1));
                }
                //Registering Address Pointers
                if (input[i].StartsWith(":"))
                {
                    //Check to make sure there is no spaces
                    if (input[i].Trim().Contains(" "))
                    {
                        Console.WriteLine("ERROR: Pointer Contains More Than One Argument. Line:\n{0}", input[i]);
                        throw new Exception("-8");
                    }
                    pointers.Add(input[i],ObjectIndex++);
                }
            }

            //Assembling
            //=====================================================

            /*
             * As explained in the documentation, instructions and data will be added to the file in the order they are inside the input file. 
             * The JMP instruction is added at the end but space is allocated right away.
             * 
             * 
             */
            Console.WriteLine("    Generating Machine Code");
            //Allocate main jump if required
            if (!isLib)
            {
                lib.code.Add(0);
                lib.code.Add(0);
            }


            //State
            bool definingFunction = false; // whether or not we are in a function block.
            string functionName = ""; //The name of the function we are defining
            int functionStartIndex = 0; //This is the address that the function begins at in the library code list.

            for(int i = 0; i < input.Length; i++)
            {
                string line = input[i];
                if (!definingFunction) //Not Defining
                {
                    //Data Segment
                    if (line.StartsWith("data")){


                        //Add the data location to the library
                        lib.internalVariables.Add(variables[line.Split(" ")[1]], lib.code.Count);


                        //Strings
                        if (line.Split(' ')[2].StartsWith("\""))
                        {
                            //Check if the string is closed correctly
                            if (!line.EndsWith("\""))
                            {
                                Console.WriteLine("ERROR: String Literal Not Closed Correctly Line:\n{0}", line.Substring(line.IndexOf("\"")));
                                throw new Exception("-4");
                            }

                            //Extract the data
                            string data = line.Substring(line.IndexOf("\""));
                            data = data.Substring(0, data.Length - 1);

                            //Replace \n with new line
                            data = data.Replace("\\n", "\n");
                            //Replace \" with "
                            data = data.Replace("\\\"", "\"");
                            //Replace \\ with \
                            data = data.Replace("\\\\", "\\");


                            
                            //Add the data
                            foreach(char c in data)
                            {
                                ushort v = c;
                                if(v > 128)
                                {
                                    Console.WriteLine("WARNING: String contains non ASCII character: \"{0}\"");
                                }
                                lib.code.Add(v);
                            }
                            //Null Terminate the string
                            lib.code.Add(0);

                        }

                        //Array of integers
                        if (line.Contains("{"))
                        {

                            //Check to make sure the array is terminated
                            if (!line.Contains("}"))
                            {
                                Console.WriteLine("ERROR: Array Not Properly Closed Line:\n{0}", line);
                                throw new Exception("-5");
                            }

                            //Extract Data
                            string data = line.Substring('{');
                            data = data.Substring(0, data.Length - 1);

                            
                            //Parse Values
                            foreach(string v in data.Split(','))
                            {
                                Int16 converted;
                                try
                                {
                                    converted = Int16.Parse(v.Trim());
                                }catch(Exception e)
                                {
                                    Console.WriteLine("ERROR: Could not parse integer. Value:{0} Error:\n{1}\nLine:\n{2}", v, e.Message,line);
                                    throw new Exception("-6");
                                    
                                }

                                //Add The value to the compiled code
                                lib.code.Add(unchecked((ushort)converted));

                            }

                        }

                        //Simple Integer
                        if(!line.Contains("{") && !line.Contains("\""))
                        {
                            //Check to make sure there is only two value and they exist.
                            if (line.Split(" ").Length != 4) 
                            {
                                Console.WriteLine("ERROR: Incorrect Number of Values to Parse, Line:\n{0}", line);
                                throw new Exception("-7");
                            }

                            //Parse the number
                            short converted;
                            int length;
                            try
                            {
                                converted = Int16.Parse(line.Split(" ")[3].Trim());
                                length = Int16.Parse(line.Split(" ")[2].Trim());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("ERROR: Could not parse integer. Value:{0} {1} Error:\n{2}\nLine:\n{3}", line.Split(" ")[2].Trim(), line.Split(" ")[3].Trim(), e.Message,line);
                                throw new Exception("-6");
                            }

                            //Add the value to the library
                            for(int j = 0; j < length; j++)
                            {
                                lib.code.Add(unchecked((ushort)converted));
                            }

                        }


                    }
                    //Using statement
                    if (line.StartsWith("using"))
                    {
                        //does Nothing for now, this was handled during id creation
                    }
                    //Define statement
                    if (line.StartsWith("define"))
                    {

                        //We know the define line is correct since we already generated an ID for it, so we do not have to check that it is formatted correctly.

                        definingFunction = true;
                        functionName = line.Split(" ")[1];
                        functionStartIndex = lib.code.Count;

                        lib.internalAddresses.Add(functions[functionName], functionStartIndex);
                    }
                }
                else //Defining a function
                {
                    //This is where we start turning instructions into machine code.
                    //check to see if it's the end

                    //End Of Function Line
                    if(line == "end")
                    {
                        definingFunction = false;
                    }
                    //Location Pointer Line
                     else if (line.StartsWith(":"))
                    {
                        //Register the address
                        lib.internalAddresses.Add(pointers[line], lib.code.Count);
                    }
                    //Instruction Line
                    else 
                    {
                        ushort instruction = 0;
                        int attachedData = 0;
                        bool hasAttached = false;

                        //Make sure that there is at most 2 arguments
                        if(line.Split(" ").Length > 3)
                        {
                            Console.WriteLine("ERROR: Too Many Arguments. Line:\n{0}", line);
                            throw new Exception("-10");
                        }
                        //Make sure the instruction exists
                        if(!Tables.instructionTable.ContainsKey(line.Split(" ")[0]))
                        {
                            Console.WriteLine("ERROR: Invalid Instruction. Line:\n{0}", line);
                            throw new Exception("-11");
                        }

                        //add the instruction ID
                        instruction += (ushort)(Tables.instructionTable[line.Split(" ")[0]] << 12);

                        //Argument 1
                        if(line.Split(" ").Length > 1)
                        {
                            string arg = line.Split(" ")[1];

                            //Is it a relative value
                            if (arg.StartsWith('['))
                            {
                                //check to make sure it ends with ]
                                if (!arg.EndsWith(']'))
                                {
                                    Console.WriteLine("ERROR: Cannot find closing bracket \']\'. Line:\n{0}", line);
                                    throw new Exception("-11");
                                }

                                //Add Type A ID
                                instruction += (ushort)(Tables.typeTable["MEM"] << 10);

                                //Remove the [] from the argument
                                arg = arg.Substring(1, arg.Length - 2);
                            }
                            else // It's a regular value
                            {
                                //Add the Type A ID
                                instruction += (ushort)(Tables.typeTable["REG"] << 10);
                            }

                            //Is it a register?
                            if (Tables.registerTable.ContainsKey(arg))
                            {
                                //Add the register ID
                                instruction += (ushort)(Tables.registerTable[arg] << 2);
                            }
                            //is it a variable?
                            else if (variables.ContainsKey(arg))
                            {
                                //Mark the instruction with an attached data bit
                                instruction += (ushort)2;
                                //attach the variable's ID
                                attachedData = variables[arg];
                                hasAttached = true;
                            }
                            //is it a pointer?
                            else if (pointers.ContainsKey(arg))
                            {
                                //Mark the instruction with an attached data bit
                                instruction += (ushort)2;
                                //attach the pointer's ID
                                attachedData = pointers[arg];
                                hasAttached = true;
                            }
                            //is it a function?
                            else if (functions.ContainsKey(arg))
                            {
                                //Mark the instruction with an attached data bit
                                instruction += (ushort)2;
                                //attach the function's ID
                                attachedData = functions[arg];
                                hasAttached = true;
                            }
                            //It's an integer
                            else
                            {
                                Int16 val = 0;
                                try
                                {
                                    val = Int16.Parse(arg);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("ERROR: Could not parse argument as Integer {0} Line:\n{1}\nError:\n{2}", arg, line, e.Message);
                                    throw new Exception("-6");
                                }
                                ushort uval = unchecked((ushort)val); //Made it unsigned
                                                                      //Mark the instruction with an attached data bit
                                instruction += (ushort)2;

                                //Attach the integer
                                hasAttached = true;
                                attachedData = uval;

                            }
                        }

                        //Argument 2
                        if (line.Split(" ").Length > 2)
                        {
                            string arg = line.Split(" ")[2];

                            //Is it a relative value
                            if (arg.StartsWith('['))
                            {
                                //check to make sure it ends with ]
                                if (!arg.EndsWith(']'))
                                {
                                    Console.WriteLine("ERROR: Cannot find closing bracket \']\'. Line:\n{0}", line);
                                    throw new Exception("-11");
                                }

                                //Add Type A ID
                                instruction += (ushort)(Tables.typeTable["MEM"] << 8);

                                //Remove the [] from the argument
                                arg = arg.Substring(1, arg.Length - 2);
                            }
                            else // It's a regular value
                            {
                                //Add the Type A ID
                                instruction += (ushort)(Tables.typeTable["REG"] << 8);
                            }

                            //Is it a register?
                            if (Tables.registerTable.ContainsKey(arg))
                            {
                                //Add the register ID
                                instruction += (ushort)(Tables.registerTable[arg] << 5);
                            }
                            //is it a variable?
                            else if (variables.ContainsKey(arg))
                            {
                                //Mark the instruction with an attached data bit
                                instruction += (ushort)2;
                                //attach the variable's ID
                                attachedData = variables[arg];
                                hasAttached = true;
                            }
                            //is it a pointer?
                            else if (pointers.ContainsKey(arg))
                            {
                                //Mark the instruction with an attached data bit
                                instruction += (ushort)2;
                                //attach the pointer's ID
                                attachedData = pointers[arg];
                                hasAttached = true;
                            }
                            //is it a function?
                            else if (functions.ContainsKey(arg))
                            {
                                //Mark the instruction with an attached data bit
                                instruction += (ushort)2;
                                //attach the function's ID
                                attachedData = functions[arg];
                                hasAttached = true;
                            }
                            //It's an integer
                            else
                            {
                                Int16 val = 0;
                                try
                                {
                                    val = Int16.Parse(arg);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("ERROR: Could not parse argument as Integer {0} Line:\n{1}\nError:\n{2}", arg, line, e.Message);
                                    throw new Exception("-6");
                                }
                                ushort uval = unchecked((ushort)val); //Made it unsigned
                                                                      //Mark the instruction with an attached data bit
                                instruction += (ushort)2;

                                //Attach the integer
                                hasAttached = true;
                                attachedData = uval;

                            }
                        }


                        //The instruction is complete
                        //Add it to the library
                        lib.code.Add(instruction);

                        //If we have attached data add that
                        if (hasAttached)
                        {
                            lib.code.Add(attachedData);
                        }

                    }


                }
            }
            lib.importedFunctions = importedFunctions;
            

            //If we are not a library we need to setup the jump at the beginning
            if (!isLib)
            {
                int functionID = functions["CPUPA.SETUP"];

                ushort instruction = 0;
                instruction += (ushort)(Tables.instructionTable["JMP"] << 12);
                instruction += (ushort)(Tables.typeTable["REG"] << 10);
                instruction += 2; //attached data
                lib.code[0] = instruction;
                lib.code[1] = functionID;
            }
            return lib;
        }

    }
}
