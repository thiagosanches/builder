using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Builder.Unit
{
    public class Helper
    {
        public const string TEMPLATE_NAME = "MyTemplateTest";
        public const string BASE_DIRECTORY = "Tests";

        public static string ExecuteCommand(string command, string builderPath)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();

                //TODO - add msbuild path to the app.config file 
                process.StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = builderPath,
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false,
                };

                process.Start();

                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                while (!process.StandardOutput.EndOfStream)
                    stringBuilder.AppendLine(process.StandardOutput.ReadLine());

                if (stringBuilder.Length == 0)
                    while (!process.StandardError.EndOfStream)
                        stringBuilder.AppendLine(process.StandardError.ReadLine());

                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CompileProject(string path)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();

                //TODO - add msbuild path to the app.config file 
                process.StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe",
                    Arguments = path,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                };

                process.Start();

                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                while (!process.StandardOutput.EndOfStream)
                    stringBuilder.AppendLine(process.StandardOutput.ReadLine());

                //TODO - refactor...
                return stringBuilder.ToString().Contains("0 Erro");
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string DefineFullQualifiedName(string solutionName, string project)
        {
            return string.Concat(solutionName, ".", project);
        }
    }
}