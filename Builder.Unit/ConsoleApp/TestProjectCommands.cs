using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Builder.ApplicationService;
using Builder.Dispatcher;
using System.Collections.Generic;
using System.IO;

namespace Builder.Unit
{
    [TestClass]
    public class TestProjectCommands
    {
        string builderPath;
        string solutionsFolders;

        [TestInitialize]
        public void Setup()
        {
            builderPath = "d:/Junior/Projetos/GITHUB.COM/thiagosanches/builder/Builder.ConsoleApp/bin/Debug/builder.exe";
            solutionsFolders = "solutions";
        }
        
        private void CleanFolders()
        {
            try
            { 
                System.IO.Directory.Delete(solutionsFolders, true);
            }
            catch (DirectoryNotFoundException ex)
            {

            }
        }

        [TestMethod]
        public void CommandProjectCreateWithController()
        {
            CleanFolders();
            var command = "create -t csharp -a MySolution -e \"MySolution.Web + (MySolution.Model + A + B + C) + (MySolution.Business + MySolution.Model + (MySolution.Dal + MySolution.Model))\"";
            var res = Helper.ExecuteCommand(command, builderPath);
            Assert.IsTrue(res.Contains("Created in"));
        }

        [TestMethod]
        public void CommandProjectCreateWithoutController()
        {
            CleanFolders();
            var command = "-t csharp -a MySolution -e \"MySolution.Web + (MySolution.Model + A + B + C) + (MySolution.Business + MySolution.Model + (MySolution.Dal + MySolution.Model))\"";
            var res = Helper.ExecuteCommand(command, builderPath);
            Assert.IsTrue(res.Contains("Created in"));
        }

        [TestMethod]
        public void CommandProjectShowExpression()
        {
            // Create project
            this.CommandProjectCreateWithoutController();

            // Show expression
            var command = @"show project expression -t csharp -f ""solutions\MySolution\MySolution.Web\MySolution.Web.csproj""";
            var res = Helper.ExecuteCommand(command, builderPath);
            Assert.IsTrue(res.Contains("MySolution.Web + (MySolution.Model + A + B + C) + (MySolution.Business + MySolution.Model + (MySolution.Dal + MySolution.Model))"));
        }

        [TestMethod]
        public void CommandProjectShowExpressionFullTree()
        {
            // Create project
            this.CommandProjectCreateWithoutController();

            // Show expression
            var command = @"show project expression --fulltree -t csharp -f ""solutions\MySolution\MySolution.Web\MySolution.Web.csproj""";
            var res = Helper.ExecuteCommand(command, builderPath);
            Assert.IsTrue(res.Contains("MySolution.Web + (MySolution.Model + A + B + C) + (MySolution.Business + (MySolution.Model + A + B + C) + (MySolution.Dal + (MySolution.Model + A + B + C)))"));
        }

        [TestMethod]
        public void CommandProjectShowHierarchy()
        {
            // Create project
            this.CommandProjectCreateWithoutController();

            // Show expression
            var command = @"show project hierarchy -t csharp -f ""solutions\MySolution\MySolution.Web\MySolution.Web.csproj""";
            var res = Helper.ExecuteCommand(command, builderPath);
            var resultExpected = "MySolution.Web (count refs: 2, count inverse refs:0)\r\n" +
                                 "...MySolution.Model (count refs: 3, count inverse refs:3)\r\n" +
                                 "......A (count refs: 0, count inverse refs:1)\r\n" +
                                 "......B (count refs: 0, count inverse refs:1)\r\n" +
                                 "......C (count refs: 0, count inverse refs:1)\r\n" +
                                 "...MySolution.Business (count refs: 2, count inverse refs:1)\r\n" +
                                 "......MySolution.Model*\r\n" +
                                 "......MySolution.Dal (count refs: 1, count inverse refs:1)\r\n" +
                                 ".........MySolution.Model*\r\n";

            Assert.IsTrue(res.Contains(resultExpected));
        }

        [TestMethod]
        public void CommandProjectShowHierarchyFullTree()
        {
            // Create project
            this.CommandProjectCreateWithoutController();

            // Show expression
            var command = @"show project hierarchy --fulltree -t csharp -f ""solutions\MySolution\MySolution.Web\MySolution.Web.csproj""";
            var res = Helper.ExecuteCommand(command, builderPath);
            var resultExpected = "MySolution.Web (count refs: 2, count inverse refs:0)\r\n" +
                                 "...MySolution.Model (count refs: 3, count inverse refs:3)\r\n" +
                                 "......A (count refs: 0, count inverse refs:1)\r\n" +
                                 "......B (count refs: 0, count inverse refs:1)\r\n" +
                                 "......C (count refs: 0, count inverse refs:1)\r\n" +
                                 "...MySolution.Business (count refs: 2, count inverse refs:1)\r\n" +
                                 "......MySolution.Model (count refs: 3, count inverse refs:3)\r\n" +
                                 ".........A (count refs: 0, count inverse refs:1)\r\n" +
                                 ".........B (count refs: 0, count inverse refs:1)\r\n" +
                                 ".........C (count refs: 0, count inverse refs:1)\r\n" +
                                 "......MySolution.Dal (count refs: 1, count inverse refs:1)\r\n" +
                                 ".........MySolution.Model (count refs: 3, count inverse refs:3)\r\n" +
                                 "............A (count refs: 0, count inverse refs:1)\r\n" +
                                 "............B (count refs: 0, count inverse refs:1)\r\n" +
                                 "............C (count refs: 0, count inverse refs:1)\r\n";

            Assert.IsTrue(res.Contains(resultExpected));
        }

        [TestMethod]
        public void CommandProjectShowHierarchyInverse()
        {
            // Create project
            this.CommandProjectCreateWithoutController();

            // Show expression
            var command = @"show project hierarchy inverse -t csharp -f ""solutions\MySolution\MySolution.Web\MySolution.Web.csproj""";
            var res = Helper.ExecuteCommand(command, builderPath);
            var resultExpected = "[any project references this project]" + "\r\n" +
                                 "...MySolution.Web" + "\r\n" +
                                 "\r\n" +
                                 "------------------------" + "\r\n" +
                                 "\r\n" +
                                 "MySolution.Web" + "\r\n" +
                                 "MySolution.Business" + "\r\n" +
                                 "MySolution.Dal" + "\r\n" +
                                 "...MySolution.Model" + "\r\n" +
                                 "\r\n" +
                                 "------------------------" + "\r\n" +
                                 "\r\n" +
                                 "MySolution.Model" + "\r\n" +
                                 "...A" + "\r\n" +
                                 "\r\n" +
                                 "------------------------" + "\r\n" +
                                 "\r\n" +
                                 "MySolution.Model" + "\r\n" +
                                 "...B" + "\r\n" +
                                 "\r\n" +
                                 "------------------------" + "\r\n" +
                                 "\r\n" +
                                 "MySolution.Model" + "\r\n" +
                                 "...C" + "\r\n" +
                                 "\r\n" +
                                 "------------------------" + "\r\n" +
                                 "\r\n" +
                                 "MySolution.Web" + "\r\n" +
                                 "...MySolution.Business" + "\r\n" +
                                 "\r\n" +
                                 "------------------------" + "\r\n" +
                                 "\r\n" +
                                 "MySolution.Business" + "\r\n" +
                                 "...MySolution.Dal" + "\r\n" +
                                 "\r\n" +
                                 "------------------------";

            Assert.IsTrue(res.Contains(resultExpected));
        }
                
        
    }
}