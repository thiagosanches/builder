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
            var controller = "";
            if (args.Length > 0 && args[0][0] != '-')
            {
                controller = args[0];
                args = args.Skip(1).ToArray();
            }

            if (controller == "" || controller == "create")
            {
                #region Create controller

                var builderOptions = new CommandCreateParams();
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
                        app.ProjectCollection.ApplyExpression(builderOptions.Expression);
                    }

                    if (app.ProjectCollection.Projects.Count() > 0)
                    {
                        builderService.Builder(builderOptions.TemplateName, app);
                    }
                    else
                    {
                        this.WriteLog(verbose, "No project was specified for creation", true);
                    }
                }

                #endregion
            }
            else if (controller == "show")
            {
                #region Show controller

                var builderOptions = new CommandShowParams();
                if (CommandLine.Parser.Default.ParseArguments(args, builderOptions))
                {
                    bool verbose = builderOptions.Verbose;
                    this.WriteLog(verbose, string.Format("Starting parse project to expression in path '{0}'", builderOptions.File));
                    var projectFactory = new ProjectLoader(builderOptions.File);
                    var project = projectFactory.LoadProject();

                    if (builderOptions.IsExpression)
                    {
                        var resExpression = project.ToStringExpression(builderOptions.FullTree);
                        System.Console.WriteLine("");
                        System.Console.WriteLine("{0}", resExpression);
                    }
                    else if (builderOptions.IsHierarchyInverse)
                    {
                        var resHierarchyInverse = project.ToStringHierarchyInverse();
                        System.Console.WriteLine("");
                        System.Console.WriteLine("{0}", resHierarchyInverse);
                    }
                    else
                    {
                        var resHierarchy = project.ToStringHierarchy(builderOptions.FullTree);
                        System.Console.WriteLine("");
                        System.Console.WriteLine("{0}", resHierarchy);
                    }
                }

                #endregion
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
