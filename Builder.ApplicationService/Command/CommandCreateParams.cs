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
    public class CommandCreateParams
    {
        [Option('v', "verbose", DefaultValue = true, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('a', "application", HelpText = "Determines the application name", Required=true)]
        public string Namespace { get; set; }

        [Option('t', "template", HelpText = "Determines the template name for use with base the code output", Required = true)]
        public string TemplateName { get; set; }

        [Option('o', "output", HelpText = "Determines the output dir", Required = false, DefaultValue = "solutions")]
        public string OutputDir { get; set; }

        [Option('e', "expression", HelpText = "Specify builder expression to create or edit your application", Required = false)]
        public string Expression { get; set; }

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
