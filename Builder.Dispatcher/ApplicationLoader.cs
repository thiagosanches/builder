using Microsoft.Build.Evaluation;
using NCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Builder.Dispatcher
{
    public class ApplicationLoader
    {
        public ApplicationLoader(string path)
        {
            var Content = System.IO.File.ReadAllText(path);
            Regex projReg = new Regex(
                "Project\\(\"\\{[\\w-]*\\}\"\\) = \"([\\w _]*.*)\", \"(.*\\.(cs|vcx|vb)proj)\""
                , RegexOptions.Compiled);
            var matches = projReg.Matches(Content).Cast<Match>();
            var Projects = matches.Select(x => x.Groups[2].Value).ToList();
            for (int i = 0; i < Projects.Count; ++i)
            {
                if (!Path.IsPathRooted(Projects[i]))
                    Projects[i] = Path.Combine(Path.GetDirectoryName(path),
                        Projects[i]);
                Projects[i] = Path.GetFullPath(Projects[i]);
            }
        }

        public object Load()
        {
            throw new NotImplementedException();
        }
    }
}
