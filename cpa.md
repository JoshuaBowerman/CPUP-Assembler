# CPUP Assembly

This is the details on the CPUP assembly language.
The Main function is Main
You are expected to push any registers you use when starting a function and popping them when done.

## Comments

Comments must be on their own line, they are started with the `//` keyword.

**Example:**
`//This is a Comment`

## Functions

To define a function use the following syntax:
`define name`
The following lines should contain instructions.
Then when done a function end it using
`end`

It is standard practice to prepend function names with a program specific id. you can use anything for an id. Generally follow the format `ID.Function`.

**Example:**
let's say you are writing the standard library and want to create a function called `SerialWrite`. Rather than naming the function simply `SerialWrite` you should choose an identifier for the library, for the standard library you might choose `std` so you should name the function `std.SerialWrite`. This ensures that with larger programs function names do not collide.

## Location Pointers

To create a pointer to a particular location in code you define it with `:` this will be replaced internally with a `.` so make sure your pointer names don't collide with your other names.

**Example:**
If you define
`:abcd`
You later could
`JMP :abcd`

## Shorthand For Small Jumps

**This is not recommended and can break your program in wierd ways**
if you just want to jump over a line or back a few instructions you can use the following. it is not recommended however and was only implemented for use in translated instructions.
**NOTE: This will not work if trying to jump over translated instructions.**

Jump fowards two instructions:
`JMP @+2`
Jump back two instructions:
`JMP @-2`

## Data
to define a data address you use the following syntax
`data .name size  value`
all variables shoudl start with a a `.` and have no spaces. size is in words. the variable will then point to the first word of this data.
variables cannot be declared inside a function. value is optional.

value can be an integer or a list of integers in this format
`{1,2,3,4,5,}`
It can also be a string of characters.

## Exposing Functions

To expose a function add `expose name` to the top of the file.

## Importing Functions

To use a function you plan on linking later add a `using` command to the begining of your file. 

**Example:**
If you wanted to use the `write` function of a library you intend to link later you would add the following to the begining of your file.

`using write`

## Calling Functions

To use a function you would write `Call name`

## Instructions

The following two lists contain all the instructions supported. They are split into two lists because the translated instructions are not actually supported by the cpu but are instead converted into a few instructions that do the same thing.

### Native Instructions

Intructions | Description
----------- | -----------
NOP | Do nothing for 1 cycle.
MOV | Move data from arg1 to arg2
JMP | Move to the passed address and begin execution
JE | Used with `CMP`, jump if equal.
JL | Jump if less than.
JG | Jump if greater.
JLE | Jump if less or equals.
JGE | Jump if greater or equals.
ADD | ADD two numbers, result is placed in second argument
SUB | Subract arg1 from arg2, arg2 = result
MUL | arg2 = arg1 * arg2
DIV | arg2 = arg2 / arg1
COM | See [IO Control Unit](https://github.com/JoshuaBowerman/CPUP/blob/main/io.md)
JER | Jump if there is an error bit set.
MOD | arg2 = arg2 mod arg1

### Translated Instructions

Intructions | Description
----------- | -----------
PUSH | Push a value onto the stack
POP | Pop a value off of the stack
INC | Increment Register (ONLY WORKS WITH REGISTERS)
DEC | Decrement Register (ONLY WORKS WITH REGISTERS)
CALL | Call a function
SIN | Serial Input 1 byte (Will wait for byte)
SOUT | Serial Output 1 byte (Will wait if buffer is full)
JNE | Jump if not equals.