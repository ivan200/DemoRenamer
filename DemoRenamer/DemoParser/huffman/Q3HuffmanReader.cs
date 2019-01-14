﻿using DemoRenamer.DemoParser.huffman;
using DemoRenamer.DemoParser.utils;
using System;
using System.Collections;
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

        public long readNumBits(int bits)
        {
            long value = 0;
            bool neg = bits < 0;

            if (neg)
                bits = bits * -1;

            int fragmentBits = bits & 7;

            if (fragmentBits != 0)
            {
                value = this.stream.readBits(fragmentBits);
                bits -= fragmentBits;
            }

            if (bits > 0)
            {
                long decoded = 0;
                for (int i = 0; i < bits; i += 8)
                {
                    long sym = Q3HuffmanMapper.decodeSymbol(this.stream);
                    if (sym == Constants.Q3_HUFFMAN_NYT_SYM)
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

        public long readNumber(int bits)
        {
            return bits == 8 ? Q3HuffmanMapper.decodeSymbol(this.stream) : this.readNumBits(bits);
        }

        public byte readByte()
        {
            return (byte) Q3HuffmanMapper.decodeSymbol(this.stream);
        }

        public short readShort()
        {
            return (short) this.readNumBits(16);
        }

        public int readInt()
        {
            return (int) this.readNumBits(32);
        }

        public long readLong()
        {
            return this.readNumBits(32);
        }

        public float readFloat()
        {
            return (float) Q3Utils.rawBitsToFloat(this.readNumBits(32));
        }

        public float readAngle16()
        {
            return (float) Q3Utils.SHORT2ANGLE(this.readNumBits(16));
        }


        public string readStringBase(int limit, bool stopAtNewLine)
        {
            List<char> arr = new List<char>();

            for (int i = 0; i < limit; i++)
            {
                long byte1 = Q3HuffmanMapper.decodeSymbol(this.stream);

                if (byte1 <= 0)
                    break;

                if (stopAtNewLine && byte1 == 0x0A)
                    break;

                // translate all fmt spec to avoid crash bugs
                // don't allow higher ascii values
                if (byte1 > 127 || byte1 == Constants.Q3_PERCENT_CHAR_BYTE)
                    byte1 = Constants.Q3_DOT_CHAR_BYTE;

                arr.Add((char)byte1);
                //arr[] = byte1;
            }

            return new string(arr.ToArray());
        }

        public string readString()
        {
            return (string)this.readStringBase(Constants.Q3_MAX_STRING_CHARS, false);
        }

        public string readBigString()
        {
            return (string)this.readStringBase(Constants.Q3_BIG_INFO_STRING, false);
        }

        public string readStringLine()
        {
            return (string)this.readStringBase(Constants.Q3_MAX_STRING_CHARS, true);
        }

        public Dictionary<string, string> readServerCommand()
        {
            Dictionary<string, string> rez = new Dictionary<string, string>();
            rez.Add("sequence", this.readLong().ToString());
            rez.Add("command", this.readString());
            return rez;
        }
    }
}
