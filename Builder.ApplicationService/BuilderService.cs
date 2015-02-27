using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Builder.Core;
using System.IO;

namespace Builder.ApplicationService
{
    public class BuilderService
    {
        public void Builder(string templateLibraryName, string projectName, string baseDir)
        {
            var builder = BuilderFactory.GetFactory(templateLibraryName);
            builder.Build(baseDir, projectName);
        }

        public string GenerateOutputPath(string solutionName, string path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Environment.CurrentDirectory + @"\solutions";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            path += @"\" + solutionName;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
                throw new Exception(string.Format("Path '{0}' alredy exists.", path));

            return path;
        }
    }
}
