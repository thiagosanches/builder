using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Builder.ApplicationService;

namespace Builder.Unit
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void BuildSingleProjectBusiness()
        {
            var builderService = new BuilderService();
            var solutionName = "MyTemplateTest";
            var baseDir = builderService.GenerateOutputPath(solutionName);
            builderService.Builder("csharp", solutionName, baseDir);

            Assert.IsTrue(CompileProject(System.IO.Path.Combine(baseDir, "MyTemplateTest.Business", "MyTemplateTest.Business.csproj")));
        }

        private bool CompileProject(string path)
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
    }
}