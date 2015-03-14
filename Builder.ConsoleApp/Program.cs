using Builder.ApplicationService;
using Builder.ApplicationService.ConsoleApp;
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
                Console.ForegroundColor = ConsoleColor.Green;
                var parser = new BuilderCommandParser();
                parser.Parser(args);
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
