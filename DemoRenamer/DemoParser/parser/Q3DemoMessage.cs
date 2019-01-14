using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRenamer.DemoParser
{
    class Q3DemoMessage
    {
        public byte[] sequence;
        public int size;
        public byte[] data;

        public Q3DemoMessage(byte[] sequence, int size) {
            this.sequence = sequence;
            this.size = size;
        }
    }
}
