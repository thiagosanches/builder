using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Builder.ApplicationService;
using Builder.Dispatcher;

namespace Builder.Unit
{
    [TestClass]
    public class BuilderTests
    {
        private BuilderService BuilderService;
        private string SolutionName;
        private string baseDirectory;
        private const string CSHARP = "csharp";

        [TestInitialize]
        public void Setup()
        {
            BuilderService = new BuilderService();
            SolutionName = Helper.TEMPLATE_NAME;
            baseDirectory = Helper.BASE_DIRECTORY;
        }

        [TestCleanup] 
        public void CleanAllDirectories()
        {
            System.IO.Directory.Delete(baseDirectory, true);
        }

        [TestMethod]
        public void BuildSingleProjectBusiness()
        {
            var baseDir = BuilderService.GenerateOutputPath(SolutionName, baseDirectory);
            var application = new Application(SolutionName, baseDir, Guid.NewGuid().ToString().ToUpper());
            ExpressionFactory.LoadApplicationByExpression(application, "MyTemplateTest.Business");
            BuilderService.Builder(CSHARP, application);

            Assert.IsTrue(Helper.CompileProject(System.IO.Path.Combine(baseDir,
                Helper.DefineFullQualifiedName(SolutionName, "Business"),
                "MyTemplateTest.Business.csproj")));
        }

        [TestMethod]
        public void BuildSingleProjectData()
        {
            var baseDir = BuilderService.GenerateOutputPath(SolutionName, baseDirectory);
            var application = new Application(SolutionName, baseDir, Guid.NewGuid().ToString().ToUpper());
            ExpressionFactory.LoadApplicationByExpression(application, "MyTemplateTest.Data");
            BuilderService.Builder(CSHARP, application);

            Assert.IsTrue(Helper.CompileProject(System.IO.Path.Combine(baseDir,
                Helper.DefineFullQualifiedName(SolutionName, "Data"),
                "MyTemplateTest.Data.csproj")));
        }

        [TestMethod]
        public void BuildSingleProjectModel()
        {
            var baseDir = BuilderService.GenerateOutputPath(SolutionName, baseDirectory);
            var application = new Application(SolutionName, baseDir, Guid.NewGuid().ToString().ToUpper());
            ExpressionFactory.LoadApplicationByExpression(application, "MyTemplateTest.Model");
            BuilderService.Builder(CSHARP, application);

            Assert.IsTrue(Helper.CompileProject(System.IO.Path.Combine(baseDir,
                Helper.DefineFullQualifiedName(SolutionName, "Model"),
                "MyTemplateTest.Model.csproj")));
        }

        [TestMethod]
        public void BuildTwoProjectWithReferences()
        {
            var baseDir = BuilderService.GenerateOutputPath(SolutionName, baseDirectory);
            var application = new Application(SolutionName, baseDir, Guid.NewGuid().ToString().ToUpper());
            ExpressionFactory.LoadApplicationByExpression(application, "MyTemplateTest.Project1 + MyTemplateTest.Project2 + (A+MyTemplateTest.Project2)");
            BuilderService.Builder(CSHARP, application);

            Assert.IsTrue(Helper.CompileProject(System.IO.Path.Combine(baseDir,
                Helper.DefineFullQualifiedName(SolutionName, "Project1"),
                "MyTemplateTest.Project1.csproj")));
        }
    }
}