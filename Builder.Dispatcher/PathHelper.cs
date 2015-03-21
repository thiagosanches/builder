using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Builder.Dispatcher
{
    class PathHelper
    {
        public static string MakeAbsolute(string absolutePathRef, string relativePath)
        {
            absolutePathRef = Path.GetDirectoryName(absolutePathRef);
            return Path.GetFullPath(Path.Combine(absolutePathRef, relativePath));
        }
    }
}
