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

        public string GenerateOutputPath(string baseDir, string solutionName, string path = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Path.Combine(Environment.CurrentDirectory, baseDir);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            path +=  @"\" + solutionName;         
            return path;
        }

        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
                throw new Exception(string.Format("Path '{0}' already exists.", path));
        }
    }
}