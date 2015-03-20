using Builder.Dispatcher;
using Builder.Template;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.ApplicationService.Command
{
    public class CommandRequest
    {
        public void Request(string[] args)
        {
            var builderOptions = new CommandParams();
            if (CommandLine.Parser.Default.ParseArguments(args, builderOptions))
            {
                bool verbose = builderOptions.Verbose;

                this.WriteLog(verbose, string.Format("Start generation for application name '{0}' with template '{1} in folder {2}.'", builderOptions.Namespace, builderOptions.TemplateName, builderOptions.OutputDir));

                var builderService = new BuilderService();
                var baseDir = builderService.GenerateOutputPath(builderOptions.Namespace, builderOptions.OutputDir);
                var app = new Application(builderOptions.Namespace, baseDir, Guid.NewGuid().ToString().ToUpper());

                if (!string.IsNullOrWhiteSpace(builderOptions.Expression))
                {
                    this.WriteLog(verbose, string.Format("Parsing project references with expression '{0}'", builderOptions.Expression));
                    ExpressionFactory.LoadApplicationByExpression(app, builderOptions.Expression);
                }

                if (app.Projects.Count > 0)
                { 
                    builderService.Builder(builderOptions.TemplateName, app);
                }
                else 
                {
                    this.WriteLog(verbose, "No project was specified for creation", true);
                }
            }
        }

        private void WriteLog(bool showLog, string log, bool error = false) 
        {
            if (error)
                Console.ForegroundColor = ConsoleColor.Red;
            if (showLog)
                Console.WriteLine(log);
            if (error)
                Console.ResetColor();
        }
    }
}
