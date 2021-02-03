using System;
using System.Collections.Generic;
using System.Text;

namespace CPUPLinker.Format
{
    class BIN : IFormat
    {
        public const string fileType = "bin";

        public BIN()
        {

        }

        public byte[] getOutput(ushort[] machineCode)
        {

            throw new NotImplementedException();
        }
    }
}
