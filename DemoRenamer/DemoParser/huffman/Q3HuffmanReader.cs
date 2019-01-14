using DemoRenamer.DemoParser.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRenamer.DemoParser
{
    class Q3HuffmanReader
    {
        private BitStreamReader stream = null;

        /**
         * HFReader constructor.
         */
        public Q3HuffmanReader(byte[] buffer)
        {
            this.stream = new BitStreamReader(buffer);
        }

        public bool isEOD()
        {
            return (bool)this.stream.isEOD();
        }

        public function readNumBits(bits)
        {
            value = 0;
            neg = bits < 0;

            if (neg)
                bits = bits * -1;

            fragmentBits = bits & 7;

            if (fragmentBits != 0)
            {
                value = this.stream.readBits(fragmentBits);
                bits -= fragmentBits;
            }

            if (bits > 0)
            {
                decoded = 0;
                for (i = 0; i < bits; i += 8)
                {
                    sym = Q3HuffmanMapper.decodeSymbol(this.stream);
                    if (sym == Q3_HUFFMAN_NYT_SYM)
                        return -1;

                    decoded |= (sym << i);
                }

                if (fragmentBits > 0)
                    decoded <<= fragmentBits;

                value |= decoded;
            }

            if (neg)
            {
                if ((value & (1 << (bits - 1))) != 0)
                {
                    value |= -1 ^ ((1 << bits) - 1);
                }
            }

            return value;
        }

        public function readNumber(bits)
        {
            return bits == 8 ? (int)Q3HuffmanMapper.decodeSymbol(this.stream) : (int)this.readNumBits(bits);
        }

        public function readByte()
        {
            return (int)Q3HuffmanMapper.decodeSymbol(this.stream);
        }

        public function readShort()
        {
            return (int)this.readNumBits(16);
        }

        public function readInt()
        {
            return (int)this.readNumBits(32);
        }

        public function readLong()
        {
            return (int)this.readNumBits(32);
        }

        public function readFloat()
        {
            return (float)Q3Utils.rawBitsToFloat(this.readNumBits(32));
        }

        public function readAngle16()
        {
            return (float)Q3Utils.SHORT2ANGLE(this.readNumBits(16));
        }


        public function readStringBase(limit, stopAtNewLine)
        {
            arr = array();
            for (i = 0; i < limit; i++)
            {
                byte = Q3HuffmanMapper.decodeSymbol(this.stream);

                if (byte <= 0)
                    break;

                if (stopAtNewLine && byte == 0x0A)
                    break;

                // translate all fmt spec to avoid crash bugs
                // don't allow higher ascii values
                if (byte > 127 || byte == Q3_PERCENT_CHAR_BYTE)
                    byte = Q3_DOT_CHAR_BYTE;

                arr[] = byte;
            }

            return (string)call_user_func_array("pack", array_merge(array("C*"), arr));
        }

        public function readString()
        {
            return (string)this.readStringBase(Q3_MAX_STRING_CHARS, false);
        }

        public function readBigString()
        {
            return (string)this.readStringBase(Q3_BIG_INFO_STRING, false);
        }

        public function readStringLine()
        {
            return (string)this.readStringBase(Q3_MAX_STRING_CHARS, true);
        }

        public function readServerCommand()
        {
            return array(
                    'sequence' => this.readLong(),
                    'command' => this.readString()
                );
        }
    }
}
