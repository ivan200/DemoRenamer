using System;
using System.Collections.Generic;
using System.Text;

namespace DemoRenamer
{
    class Q3DemoParser
    {
        private string file_name;

        /**
        * Q3DemoParser constructor.
        * @param string file_name - name of demo-file
        */
        public Q3DemoParser(string file_name) {
            this.file_name = file_name;
        }

        public void parseConfig()
        {
            var msgParser = new Q3DemoConfigParser();
            $this->doParse($msgParser);
            return $msgParser->hasConfigs() ? $msgParser->getRawConfigs() : NULL;
        }
    }
}
