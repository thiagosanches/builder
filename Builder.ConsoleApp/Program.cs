using Builder.ApplicationService;
using Builder.ApplicationService.Command;
using Builder.Dispatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.ConsoleApp
{
    class Program
    {
        private const int CONSOLE_SUCCESS = 0;
        private const int CONSOLE_ERROR = 1;

        static int Main(string[] args)
        {
            int code = CONSOLE_SUCCESS;
            try
            {
                // Wait to attach process
                // Console.ReadLine();

                Console.ForegroundColor = ConsoleColor.Green;
                var parser = new CommandRequest();

                //args = new string[] {"-t csharp", "-a MySolution",  "-e \"MySolution.Web + MySolution.Model + (MySolution.Business + MySolution.Model + (MySolution.Dal + MySolution.Model))\""};
                //var pathFileCsProj = @"D:\Junior\Projetos\GITHUB.COM\thiagosanches\builder\Builder.ConsoleApp\bin\Debug\solutions\MySolution\MySolution.Web\MySolution.Web.csproj";
                //args = new string[] { "show", "-e", "-t", "csharp", "-f", pathFileCsProj };
                //args = new string[] { "show", "", "-t", "csharp", "-f", pathFileCsProj };
                //args = new string[] { "show", "-t csharp", "-f", string.Format("\"{0}\"", pathFileCsProj), "-p" };

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
