using System;
using System.Collections.Generic;
using System.Text;

namespace CPUPA
{
    class Tables
    {
        //This table contains the translations from instructions into the instruction IDs.
        //The value isn't the entire machine instruction, it's just the backbone of the instruction
        public static Dictionary<string, ushort> instructionTable = new Dictionary<string, ushort>()
        {
            { "NOP", 0b0000},
            { "MOV", 0b0001},
            { "JMP", 0b0010},
            { "JE",  0b0011},
            { "JL",  0b0100},
            { "JG",  0b0101},
            { "JLE", 0b0110},
            { "JGE", 0b0111},
            { "ADD", 0b1000},
            { "SUB", 0b1001},
            { "MUL", 0b1010},
            { "DIV", 0b1011},
            { "COM", 0b1100},
            { "JER", 0b1101},
            { "MOD", 0b1110}
        };

        //This table contains types and type IDs
        //They are largely redundant as i reduced the types from 4 to 2 so it could have ust been one bit
        //instead its 2 bits
        public static Dictionary<string, ushort> typeTable = new Dictionary<string, ushort>()
        {
            { "REG", 0b00 },
            { "MEM", 0b01 }
        };

        //This is the register to register id table
        public static Dictionary<string, ushort> registerTable = new Dictionary<string, ushort>()
        {
            {"A",  0b000 },
            {"B",  0b001 },
            {"C",  0b010 },
            {"P",  0b011 },
            {"S",  0b100 },
            {"ST", 0b101 },
            {"IO", 0b110 },
            {"ER", 0b111 }
        };

        //These lines are inserted at the end of the file.
        //They contain data used for function jumping. (Temporary Data)
        public static List<String> defaultData = new List<String>()
        {
            "data .CPUPA.TEMP 1 0"
        };

        //This data is added only if the program is not a library
        //$$ will be replaced by the stack size
        public static List<String> defaultProgramData = new List<String>()
        {
            "data .CPUPA.STACK $$ 0"
        };


        //This is the code to JMP back after a function
        //This is whats called at the end of a function
        public static List<String> functionEnd = new List<string>()
        {
            "MOV A [.CPUPA.TEMP]",
            "POP A",
            "ADD 3 A",
            "JMP A",
        };

        //This is the code that is run at the begining of a program. it sets up the stack.
        //If it is being assembled as a library this code is ommitted
        // $$ is the stack size
        // The function name should be CPUP.SETUP
        public static List<string> setupCode = new List<string>()
        {
            "define CPUPA.SETUP",
            "MOV .CPUPA.STACK ST",
            "CALL MAIN",
            ":CPUPA.HLT",
            "JMP :CPUPA.HLT",
            "end"
        };

        /* This table contains instruction translations. these are instructions that dont have actual instructions on the cpu.
         * They are instead translated into appropriatte assembly by the linter.
         * 
         * $$ is Argument 1
         * %% is Argument 2
         * 
         */
        public static Dictionary<string, List<string>> translatedInstructions = new Dictionary<string, List<string>>()
        {
            {"PUSH", new List<string>(){
                "ADD 1 ST",
                "MOV $$ [ST]" }
            },
            {"POP", new List<string>()
            {
                "MOV [ST] $$",
                "SUB 1 ST" }
            },
            {"INC", new List<string>()
            {
                "ADD 1 $$" }
            },
            {"DEC", new List<string>()
            {
                "SUB 1 $$" }
            },
            {"CALL", new List<string>()
            {
                "ADD 1 ST",
                "MOV P [ST]",
                "JMP $$",
                "NOP",
                "MOV [.CPUPA.TEMP] A" }
            },
            {"SIN", new List<string>(){
                "COM 3",
                "MV IO $$"
            }},
            { "SOUT", new List<string>(){
                "MV $$ IO",
                "COM 2"

             } },
            { "JNE", new List<string>(){
                "JL $$",
                "JG $$"
            }}};
    }
}
