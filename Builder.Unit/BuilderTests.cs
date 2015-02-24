using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Builder.Unit
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void BuildSingleProjectBusiness()
        {
            Builder.Template.Builder builder = new Template.Builder();

            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "temporary");
            string pathPackage = builder.Build(@"..\Debug\Template\ClassLibrary.xml", path, "MyTemplateTest");

            Assert.IsTrue(CompileProject(System.IO.Path.Combine(pathPackage, "MyTemplateTest.Business", "MyTemplateTest.Business.csproj")));
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