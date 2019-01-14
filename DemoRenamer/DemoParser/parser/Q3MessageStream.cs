using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DemoRenamer.DemoParser.parser
{
    class Q3MessageStream
    {
        private Stream fileHandle = null;
        private int readBytes = 0;
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
            int bytesRead = 0;

            byte[] headerBuffer = new byte[8];
            bytesRead = fileHandle.Read(headerBuffer, readBytes, 8);
            if (bytesRead != 8) {
                return null;
            }

            this.readBytes += 8;

            int msgLength = BitConverter.ToInt32(headerBuffer, 0);

            if (msgLength == -1) {
                // a normal case, end of message-sequence
                return null;
            }

            if (msgLength < 0 || msgLength > Constants.Q3_MESSAGE_MAX_SIZE) {
                throw new Exception("Demo file is corrupted, wrong message length: {msgLength}");
            }

            var msg = new Q3DemoMessage(headerBuffer, msgLength);

            byte[] bodyBuffer = new byte[msgLength];
            bytesRead = fileHandle.Read(bodyBuffer, readBytes, msgLength);
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
