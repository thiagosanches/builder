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

        public string GenerateOutputPath(string solutionName, string outputDir)
        {
            // Path.Combine(Environment.CurrentDirectory, outputDir);
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            return Path.Combine(outputDir, solutionName);
        }
    }
}