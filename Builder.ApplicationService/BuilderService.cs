using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Builder.Core;
using System.IO;
using Builder.Dispatcher;
using Builder.Template;

namespace Builder.ApplicationService
{
    public class BuilderService
    {
        public void Builder(string templateLibraryName, Application app)
        {
            var builder = BuilderFactory.GetFactory(templateLibraryName);
            builder.Build(app);
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