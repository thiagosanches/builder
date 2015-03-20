using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Builder.Dispatcher
{
    public class File
    {
        public string Name { get; private set; }
        public string Extension { get; private set; }
        public string Content { get; private set; }
    }
}
