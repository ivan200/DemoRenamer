using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRenamer.DemoParser.utils
{
    class BitStreamReader
    {
        private int byteIdx = 0;


        // array of integers, first value have index '1'
        // this var holds result of unpack operation
        private byte[] data;

        // the number of bits in this stream
        private int bit_length;

        // cached value of integer taken from data
        private int currentBits;

        // index of bit (read position) in a virtual bit-stream
        // it'a a sequential number of reads from this stream
        private int bitIdx;


        /**
         * BitStreamReader constructor.
         * @param data assumes it's a binary string taken from 'fread' call or array of integers
         */
        public BitStreamReader(byte[] data)
        {

            //if (is_string(data))
            //{
            //    this.bit_length = strlen(data) * 8;
            //    // unpack binary string into array of integers
            //    //
            //    this.data = unpack("I*", data.str_repeat("\0", 4 - ((this.bit_length / 8) & 0x03)));
            //}
            //else if (is_array(data))
            //{
            this.bit_length = data.Length * 32;
            this.data = data;
            //}

            this.reset();
        }

        /**
         * Reset this stream. It sets read position to 0 (begin)
         */
        public void reset()
        {
            this.bitIdx = 0;
            this.byteIdx = 0;
            this.currentBits = this.data[byteIdx];
        }



        /**
         * Test if end-of-data is reached
         * @return bool return TRUE if end-of-data reached, else FALSE
         */
        public bool isEOD()
        {
            return this.bitIdx >= this.bit_length;
        }

        /**
         * Read required amount of bits (bits) from this stream.
         * Result will have all bits in right-to-left order (a normal bits order),
         * so the first read bit will be lowest
         * @param int bits amount of bits to read. value has to be in a range 1..32
         * @return int
         */
        public int readBits(int bits)
        {
            if (bits < 0 || bits > 32 || this.bitIdx + bits > this.bit_length)
                return -1;

            int value = 0;
            // bit mask to set for target value
            int setBit = 1;

            // cache read position, local variables access is much faster
            int intIdx = this.bitIdx;
            // cache curr bits
            int intBits = this.currentBits;


            // amount of bits we can read from current cached value
            int currAmount = 32 - (intIdx & 31);
            int tread = bits > currAmount ? currAmount : bits;

            bits -= tread;
            intIdx += tread;

            while (tread > 0)
            {
                if ((intBits & 1) == 1)
                    value |= setBit;

                setBit <<= 1;
                intBits >>= 1;
                --tread;
            }

            if (bits > 0)
            {
                // we have to switch to next int from data-buffer
                intBits = this.data[++byteIdx];
                intIdx += bits;

                while (bits > 0)
                {
                    if ((intBits & 1) == 1)
                        value |= setBit;

                    setBit <<= 1;
                    intBits >>= 1;
                    --bits;
                }
            }

            // write local values back
            this.currentBits = intBits;
            this.bitIdx = intIdx;
            //        echo ", in end read-pos= {intIdx} \n";

            return (int)value;
        }

        /**
         * Method read and return next bit value from this stream
         * @return int returns next bit value (0 or 1) or -1 in case end of data
         */
        public int nextBit()
        {
            if (this.bitIdx >= this.bit_length)
                return -1;

            int rez = this.currentBits & 1;
            ++this.bitIdx;

            if ((this.bitIdx & 31) == 1)
                this.currentBits >>= 1;
            else
                this.currentBits = this.data[++byteIdx];

            return (int)rez;
        }

        /**
         * It skips amount of bits
         * @param int skip value has to be in range 1..32
         * @return int returns current bit-read position of this stream
         */
        public int skipBits(int skip)
        {
            if (skip < 0 || skip > 32 || this.bitIdx + skip > this.bit_length)
                return -1;

            int currAmount = 32 - (this.bitIdx & 31);
            this.bitIdx += skip;

            if (currAmount > skip)
            {
                this.currentBits >>= skip;
            }
            else
            {
                this.currentBits = this.data[++byteIdx];
                skip -= currAmount;
                this.currentBits >>= skip;
            }

            return (int)this.bitIdx;
        }
    }
}
