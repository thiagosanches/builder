using Builder.ApplicationService;
using Builder.ApplicationService.Command;
using Builder.Dispatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Builder.ConsoleApp
{
    class Program
    {
        private const int CONSOLE_SUCCESS = 0;
        private const int CONSOLE_ERROR = 1;

        static int Main(string[] args)
        {
            Console.ReadKey();

            //new TestExpressionBuilder().TestDefault();

            //return 0;

            int code = CONSOLE_SUCCESS;
            try
            {
                //Thread.Sleep(20000);
                // Wait to attach process
                //Console.ReadLine();
                //args = new string[] { "-t", "csharp", "-a", "MySolution", "-e", "MySolution.Web + (MySolution.Model + A + B + C) + (MySolution.Business + MySolution.Model + (MySolution.Dal + MySolution.Model))" };
                var pathApp = @"D:\Junior\Projetos\GITHUB.COM\thiagosanches\builder\Builder.ConsoleApp\bin\Debug\solutions\MySolution\MySolution.sln";
                var pathFileCsProj = @"D:\Junior\Projetos\GITHUB.COM\thiagosanches\builder\Builder.ConsoleApp\bin\Debug\solutions\MySolution\MySolution.Web\MySolution.Web.csproj";
                
                //args = new string[] { "show", "-e", "-t", "csharp", "-f", pathFileCsProj };
                //args = new string[] { "show", "", "-t", "csharp", "-f", pathFileCsProj };
                //args = new string[] { "show", "-t csharp", "-f", string.Format("\"{0}\"", pathFileCsProj), "-p" };
                
                //args = new string[] { "show", "project", "hierarchy", "--fulltree", "-t", "csharp", "-f", pathFileCsProj };
                //args = new string[] { "show", "project", "hierarchy", "-t", "csharp", "-f", pathFileCsProj };

                //args = new string[] { "show", "application", "expression", "--fulltree", "-t", "csharp", "-f", pathApp };
                //args = new string[] { "show", "application", "expression", "-t", "csharp", "-f", pathApp };
                //args = new string[] { "show", "application", "hierarchy", "--fulltree", "-t", "csharp", "-f", pathApp };
                //args = new string[] { "show", "application", "hierarchy", "-t", "csharp", "-f", pathApp };
                //args = new string[] { "show", "application", "hierarchy", "inverse", "--fulltree", "-t", "csharp", "-f", pathApp };
                //args = new string[] { "show", "application", "hierarchy", "inverse", "-t", "csharp", "-f", pathApp };
                
                Console.ForegroundColor = ConsoleColor.Green;
                var parser = new CommandRequest();
                parser.Request(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    Console.WriteLine(ex.InnerException.StackTrace);
                }

                code = CONSOLE_ERROR;
            }
            finally
            {
                Console.ResetColor();
            }

            return code;
        }
    }
}
