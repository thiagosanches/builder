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
    public class TestBasicCommands
    {
        string builderPath;

        [TestInitialize]
        public void Setup()
        {
            builderPath = "d:/Junior/Projetos/GITHUB.COM/thiagosanches/builder/Builder.ConsoleApp/bin/Debug/builder.exe";
        }

        [TestMethod]
        public void CommandNotFound()
        {
            var command = "abc";
            var res = Helper.ExecuteCommand(command, builderPath);
            var resultExpected = "builder: 'abc' is not a builder command. See 'builder --help'.\r\n" +
                                 "Did you mean one of these?";
            Assert.IsTrue(res.Contains(resultExpected));
        }

        [TestMethod]
        public void CommandHelp()
        {
            var command = "--help";
            var res = Helper.ExecuteCommand(command, builderPath);
            var resultExpected = "Builder 0.0.0.1" + "\r\n" +
                                 "Copyright ©  2015";

            Assert.IsTrue(res.Contains(resultExpected));

            command = "";
            res = Helper.ExecuteCommand(command, builderPath);
            resultExpected = "Builder 0.0.0.1" + "\r\n" +
                             "Copyright ©  2015";

            Assert.IsTrue(res.Contains(resultExpected));
        }
    }
}