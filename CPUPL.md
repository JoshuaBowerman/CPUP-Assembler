#CPUP Linker
The CPUP Linker(CPUPLinker) is responsible for taking one or more `.lib` files and creating a file that is able to be executed by a CPUP Processor.
It accepts just one or multiple .LIB files, it will link these into a single program. Only one of the input files can be a Program file as this will be used as the entry point.

Format:
 CPUPA.exe \[Switches\] Input1 Input2 Input3...

## Switches
Switch | Use
------ | ---
-of=VAL | Sets the output file to the provided VAL, use double quotes for file names with spaces
-hex | Outputs a Intel Hexadecimal File
-bin | Outputs a binary file
-mif | Outputs an Intel Memory Initialization File

## Return Values

Value | Meaning
----- | -------
-1 | Invalid Switch
-2 | Cannot find input file.
-3 | More Than One Program File Found(Files containing a main function)