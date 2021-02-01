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


