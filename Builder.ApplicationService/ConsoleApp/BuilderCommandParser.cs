using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.ApplicationService.ConsoleApp
{
    public class BuilderCommandParser
    {
        public void Parser(string[] args)
        {
            var builderOptions = new BuilderOptions();
            if (CommandLine.Parser.Default.ParseArguments(args, builderOptions))
            {
                if (builderOptions.Verbose)
                    Console.WriteLine("Start generation for application name '{0}' with template '{1} in folder {2}.'", builderOptions.Namespace, builderOptions.TemplateName, builderOptions.OutputDir);

                var builderService = new BuilderService();
                var baseDir = builderService.GenerateOutputPath(builderOptions.Namespace, builderOptions.OutputDir);

                builderService.Builder(builderOptions.TemplateName, builderOptions.Namespace, baseDir);
            }
        }
    }
}
