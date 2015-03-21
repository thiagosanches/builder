using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.ApplicationService.Command
{
    /// <summary>
    /// Link: http://commandline.codeplex.com/
    /// </summary>
    public class CommandShowParams
    {
        [Option('v', "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output")]
        public bool Verbose { get; set; }

        [Option('f', "file", HelpText = "Determines the file to create the expression", Required = true)]
        public string File { get; set; }

        [Option('t', "template", HelpText = "Determines the template name", Required = true)]
        public string TemplateName { get; set; }

        [Option("expression", HelpText = "Determines that the result will be a expression, if the parameter is not passed, it will be considered as hierarchical print", Required = false, DefaultValue = false)]
        public bool IsExpression { get; set; }

        [Option("fulltree", HelpText = "Determines that the result show a full tree in output", Required = false, DefaultValue = false)]
        public bool FullTree { get; set; }

        [Option("inverse", HelpText = "Determines that the result show in output a inverse hierarchy from this project ", Required = false, DefaultValue = false)]
        public bool IsHierarchyInverse { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
            help.AdditionalNewLineAfterOption = false;
            help.AddDashesToOption = false;
            return help;
        }

    }
}
