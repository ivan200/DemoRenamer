using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DemoRenamer.DemoParser.parser
{
    class Q3MessageStream
    {
        private Stream fileHandle = null;
        private long readBytes = 0;
        private int readMessages = 0;

        /**
         * Q3DemoParser constructor.
         * @param string file_name - name of demo-file
         * @throws Exception in case file is failed to open
         */
        public Q3MessageStream(string file_name) {
            this.readBytes = 0;
            this.readMessages = 0;
            this.fileHandle = File.OpenRead(file_name);
            if (!fileHandle.CanRead) {
                throw new Exception("can't open demofile {file_name}...");
            }
        }

        /**
        * @return Q3DemoMessage return a next message buffer or null if EOD is reached
        * @throws Exception in case stream is corrupted
        */
        public Q3DemoMessage nextMessage()
        {
            int cbytes = 8;

            int bytesRead = 0;

            byte[] headerBuffer = new byte[cbytes];
            bytesRead = fileHandle.Read(headerBuffer, (int) readBytes, cbytes);
            if (bytesRead != cbytes) {
                return null;
            }
            
            this.readBytes += cbytes;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(headerBuffer);

            long msgLength = BitConverter.ToInt64(headerBuffer, 0);

            if (msgLength == -1) {
                // a normal case, end of message-sequence
                return null;
            }

            if (msgLength < 0 || msgLength > Constants.Q3_MESSAGE_MAX_SIZE) {
                throw new Exception("Demo file is corrupted, wrong message length: {msgLength}");
            }

            var msg = new Q3DemoMessage(headerBuffer, (int)msgLength);

            byte[] bodyBuffer = new byte[msgLength];
            bytesRead = fileHandle.Read(bodyBuffer, (int)readBytes, (int)msgLength);
            msg.data = bodyBuffer;

            this.readBytes += msgLength;
            this.readMessages++;

            return msg;
        }

        public void close()
        {
            if (this.fileHandle!= null) {
                fileHandle.Close();
                fileHandle = null;
            }
        }
    }
}
