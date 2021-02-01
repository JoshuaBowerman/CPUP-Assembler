# CPUP Assembler

The assembler takes `cpa` files and outputs `lib` files. it's job is to convert the assembly into machine code and IDs. there are three types of IDs; Function IDs, Variable IDs, and Pointer IDs.
These IDs are later converted into actual addresses by the linker. the lib file contains information on where the ids point to in the library file. Function IDs point to the first instruction of the function, Variable IDs point to the first word of a piece of data. and pointer IDs lead to a particular instruction.

