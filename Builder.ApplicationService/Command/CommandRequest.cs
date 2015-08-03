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
        private List<string> controllers = new List<string>();
        private bool Verbose { get; set; }

        public void Request(string[] args)
        {
            var originalArgs = args;
            args = FilterArgs(args);

            // Discover and execute command
            if (this.HasController("show"))
            { 
                if (this.HasController("project"))
                    ExecuteShowProjectCommand(args);
                else if (this.HasController("application"))
                    ExecuteShowApplicationCommand(args);
            }
            else if (controllers.Count == 0 || this.HasController("create"))
            {
                ExecuteCreateCommand(args);
            }
            else 
            {
                ShowControllerNotFound(originalArgs);
            }
        }

        private void ExecuteShowApplicationCommand(string[] args)
        {
            var builderOptions = new CommandShowParams();
            CommandLine.Parser.Default.ParseArguments(args, builderOptions);

            //if (CommandLine.Parser.Default.ParseArguments(args, builderOptions))
            {
                this.Verbose = builderOptions.Verbose;
                var filePath = builderOptions.File;

                var app = new ApplicationLoader(filePath).Load();

                if (this.HasController("expression"))
                {
                    this.WriteLog(string.Format("Showing the expression of application '{0}'", filePath));
                    var resExpression = app.ProjectCollection.ToExpression(builderOptions.FullTree);

                    System.Console.WriteLine("");
                    foreach (var exp in resExpression)
                    {
                        System.Console.WriteLine("{0}", exp.Value);
                    }
                }
                else if (this.HasController("hierarchy"))
                {
                    Dictionary<string, string> dic;

                    if (this.HasController("inverse"))
                    {
                        this.WriteLog(string.Format("Showing the inverse hierarchy of application '{0}'", filePath));
                        dic = app.ProjectCollection.ToStringHierarchyInverse();

                        System.Console.WriteLine("Parents of project: WalMartBr.Ecommerce.WS.GoogleMerchant");
                        var listRef = app.ProjectCollection.Projects.Where(f => f.Name.Contains("WalMartBr.Ecommerce.WS.GoogleMerchant")).ToList();
                        foreach (var obj in listRef)
                        {
                            foreach (var parents in obj.ProjectsReferences)
                            {
                                System.Console.WriteLine(parents.Name);
                            }
                        }

                        System.Console.WriteLine("********************************");
                        System.Console.WriteLine("Parents of project: Vtex.Practices.Search");
                        var list = app.ProjectCollection.Projects.Where(f => f.Name.Contains("Vtex.Practices.Search")).ToList();
                        foreach(var obj in list)
                        {
                            foreach (var parents in obj.ProjectsParents)
                            {
                                System.Console.WriteLine(parents.Name + " (" + parents.Path + ")");
                            }
                        }
                        System.Console.WriteLine();
                        System.Console.WriteLine("********************************");
                    }
                    else
                    {
                        this.WriteLog(string.Format("Showing the hierarchy of application '{0}'", filePath));
                        dic = app.ProjectCollection.ToStringHierarchy(builderOptions.FullTree);

                    }

                    System.Console.WriteLine("");
                    
                    foreach (var exp in dic)
                    {
                        System.Console.WriteLine("------------------ {0}", exp.Key);

                        System.Console.WriteLine();
                        System.Console.WriteLine("{0}", exp.Value);
                    }
                }
            }
        }

        private void ExecuteShowProjectCommand(string[] args)
        {
            var builderOptions = new CommandShowParams();
            if (CommandLine.Parser.Default.ParseArguments(args, builderOptions))
            {
                this.Verbose = builderOptions.Verbose;
                var filePath = builderOptions.File;

                var projectFactory = new ProjectLoader(filePath);
                var project = projectFactory.Load();

                if (this.HasController("expression"))
                {
                    this.WriteLog(string.Format("Showing the expression of project '{0}'", filePath));
                    var resExpression = project.ToStringExpression(builderOptions.FullTree);
                    System.Console.WriteLine("");
                    System.Console.WriteLine("{0}", resExpression);
                }
                else if (this.HasController("hierarchy"))
                {
                    if (this.HasController("inverse"))
                    {
                        this.WriteLog(string.Format("Showing the inverse hierarchy of project '{0}'", filePath));
                        var resHierarchyInverse = project.ToStringHierarchyInverse();
                        System.Console.WriteLine("");
                        System.Console.WriteLine("{0}", resHierarchyInverse);
                    }
                    else
                    {
                        this.WriteLog(string.Format("Showing the hierarchy of project '{0}'", filePath));
                        var resHierarchy = project.ToStringHierarchy(builderOptions.FullTree);
                        System.Console.WriteLine("");
                        System.Console.WriteLine("{0}", resHierarchy);
                    }
                }
            }
        }

        private void ExecuteCreateCommand(string[] args)
        {
            var builderOptions = new CommandCreateParams();
            if (CommandLine.Parser.Default.ParseArguments(args, builderOptions))
            {
                this.Verbose = builderOptions.Verbose;

                this.WriteLog(string.Format("Start generation for application name '{0}' with template '{1} in folder {2}.'", builderOptions.Namespace, builderOptions.TemplateName, builderOptions.OutputDir));

                var builderService = new BuilderService();
                var baseDir = builderService.GenerateOutputPath(builderOptions.Namespace, builderOptions.OutputDir);
                var app = new Application(builderOptions.Namespace, baseDir, Guid.NewGuid().ToString().ToUpper());

                if (!string.IsNullOrWhiteSpace(builderOptions.Expression))
                {
                    this.WriteLog(string.Format("Parsing project references with expression '{0}'", builderOptions.Expression));
                    app.ProjectCollection.ApplyExpression(builderOptions.Expression);
                }

                if (app.ProjectCollection.Projects.Count() > 0)
                {
                    builderService.Builder(builderOptions.TemplateName, app);
                    this.WriteLog(string.Format("Created in '{0}'", baseDir));
                }
                else
                {
                    this.WriteLog("No project was specified for creation", true);
                }
            }
        }

        /// <summary>
        /// Parse sub commands (I say too: controllers in parallel of MVC)
        /// If first char ins't a parameter delimiter "-" or string value or number,
        /// then is a sub command
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string[] FilterArgs(string[] args)
        {
            if (args.Length > 0)
            { 
                var firstChar = args[0].Trim()[0];
                while (args.Length > 0 && firstChar != '-'
                    && firstChar != '"' && firstChar != '\'' && !Char.IsNumber(firstChar))
                {
                    controllers.Add(args[0]);
                    args = args.Skip(1).ToArray();

                    if (args.Length > 0)
                        firstChar = args[0].Trim()[0];
                }
            }
            return args;
        }

        private bool HasController(string name)
        {
            return controllers.Exists(f => f.Trim().ToLower() == name.Trim().ToLower());
        }

        /// <summary>
        /// TODO: Improve this method and mecanism
        /// </summary>
        /// <param name="args"></param>
        private void ShowControllerNotFound(string[] args)
        {
            Console.WriteLine("builder: '{0}' is not a builder command. See 'builder --help'.", string.Join(" ", args));
            Console.WriteLine("Did you mean one of these?");
            Console.WriteLine("****MISSING TO IMPLEMMENT ****");
        }

        /// <summary>
        /// TODO: Improve log/verbose mecanism
        /// </summary>
        /// <param name="log"></param>
        /// <param name="error"></param>
        private void WriteLog(string log, bool error = false)
        {
            if (error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.ResetColor();
            }
            if (this.Verbose) 
            { 
                Console.WriteLine(log);
            }
        }
    }
}
