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
        static void Main(string[] args)
        {
            // Classe gerada em RunTime, esse serve ;)
            Console.WriteLine("Digite o nome do projeto:");
            var projectName = Console.ReadLine();
            
            Builder.Template.Builder builder = new Template.Builder();
            var baseDir = builder.GenerateOutputBaseDir();
            builder.Build(baseDir, projectName);

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------------------------------------------");
            Console.WriteLine("Arquivo gerado na pasta: " + baseDir);
            Console.WriteLine("----------------------------------------------------------------------------------------------------");
            Console.Read();
        }
    }
}
