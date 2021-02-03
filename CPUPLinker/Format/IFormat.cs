using System;
using System.Collections.Generic;
using System.Text;

namespace CPUPLinker.Format
{
    interface IFormat
    {
        byte[] getOutput(ushort[] machineCode);
    }
}
