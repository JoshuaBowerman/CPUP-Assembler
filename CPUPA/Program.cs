using System;

namespace CPUPA
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("CPUP Assembler");
                Console.WriteLine("Accepts a CPA file and outputs a machine code file either in bin or MIF formats.");
                Console.WriteLine("Usage: CPUPA.exe [Swtiches] Input.cpa output.bin ");
                Console.WriteLine("Switches:");
                Console.WriteLine("-M : Output file in MIF format");
                Console.WriteLine("-B : Output file in bin format");
                Console.WriteLine("");
                return 0;
            }



            return 0;
        }
    }
}
