using System;
using System.Reflection;
using Builder.ApplicationService;
using Builder.Dispatcher;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Builder.Core.Expression;

namespace Builder.ConsoleApp
{
    public class TestExpressionBuilder
    {
        string path;
        string nsTest1;
        string pathSln;

        public TestExpressionBuilder()
        {
            path = @"C:\TestsBuilder";
            nsTest1 = "NsTest1";
            pathSln = path + @"\NsTest1\" + nsTest1 + ".sln";
        }

        public void CreateNewProject(string exp)
        {
            if (!System.IO.File.Exists(pathSln))
            {
                var builderService = new BuilderService();
                var baseDir = builderService.GenerateOutputPath("NsTest1", path);
                var app = new Application("NsTest1", baseDir, Guid.NewGuid().ToString().ToUpper());
                app.ProjectCollection.ApplyExpression(exp);
                builderService.Builder("csharp", app);
            }
        }

        public void DelProject()
        {
            try
            { 
                System.IO.Directory.Delete(path + @"\" + nsTest1, true);
            }
            catch
            {

            }
        }
        
        public void TestDefault()
        {
            DelProject();
            //CreateNewProject("A + B + C + D + (E+J) + J + (I+E)");
            CreateNewProject("A + B + C + D + (X+Z) + (((E+J) + (K+E+I+P+(X+J+Y))))");
            //CreateNewProject("A + (A+C)");
            var app = new ApplicationLoader(pathSln).Load();
            var expReader = new ExpressionReader(app.ProjectCollection.GetProjectByName("A"), false);
            var a = expReader.ToExpression();
            //var res = expReader.Read( app.ProjectCollection.GetProjectByName("A"));
            //var expReader2 = new ExpressionReader(app.ProjectCollection, res);
            //var res2 = expReader2.Read(app.ProjectCollection.GetProjectByName("A"));
        }
    }
}