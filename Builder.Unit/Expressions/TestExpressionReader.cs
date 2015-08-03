using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Builder.ApplicationService;
using Builder.Dispatcher;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NCalc;
using Builder.Core.Expression;
using System.Text.RegularExpressions;

namespace Builder.Unit
{
    [TestClass]
    public class TestExpressionReader
    {
        #region Helpers

        string path;
        string nsTest1;
        string pathSln;

        public class TestCaseDependenceExpression
        {
            public string Description { get; set; }
            public string Input { get; set; }
            public string OutputExpected { get; set; }
            public string OutputExpectedFullTree { get; set; }
            public string ProjectMain { get; set; }

            public TestCaseDependenceExpression()
            {
            }
        }

        [TestInitialize]
        public void Setup()
        {
            path = @"C:\TestsBuilder";
            nsTest1 = "NsTest1";
            pathSln = path + @"\NsTest1\" + nsTest1 + ".sln";
        }

        [TestMethod]
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

        [TestMethod]
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

        /// <summary>
        /// Test if method ToExpression is returned a expected result
        /// Test if method Debug is returned a expected result
        /// Test if method GetTokens return a correct sequence and the specials tokens [+, (, )] are of the unique instance
        /// </summary>
        /// <param name="outputExpected"></param>
        /// <param name="expReader"></param>
        public void TestCommomCenaries(TestCaseDependenceExpression test, ExpressionReader expReader)
        {
            var output = test.OutputExpected;
            if (output == null)
                output = test.Input;

            if (expReader.ShowFullTree && !string.IsNullOrWhiteSpace(test.OutputExpectedFullTree))
                output = test.OutputExpectedFullTree;

            if (string.IsNullOrWhiteSpace(output))
                throw new Exception("Output is null");

            output = output.Replace(" ", "");
            output = output.Replace("+", " + ");
            output = output.Replace("-", " - ");

            Assert.IsTrue(output == expReader.ToExpression(), "Test expression: compare output - " + test.Description );

            var debug = expReader.Debug();
            var debugSplit = debug.Split(new char[] { '\r', '\n' });
            debugSplit = debugSplit.Where(f => f != "").ToArray();

            var regEx = @"[a-zA-Z\._]+|\(|\)|\+|\-";

            // Testing debug
            string[] outputExpectedSplit = Regex.Matches(output, regEx).Cast<Match>().Select(m => m.Value).ToArray();
            for (var i = 0; i < outputExpectedSplit.Length; i++)
            {
                string[] debugSplit2 = Regex.Matches(debugSplit[i], regEx).Cast<Match>().Select(m => m.Value).ToArray();
                Assert.IsTrue(outputExpectedSplit[i] == debugSplit2[0], "Test debug: compare output ´- " + test.Description);
            }

            var tokens = expReader.GetTokens();
            for (var i = 0; i < outputExpectedSplit.Length; i++)
            {
                if (tokens[i].TokenValue is TokenValuePlus)
                    Assert.IsTrue(tokens[i].TokenValue == TokenValuePlus.Instance, "Test get token: plus instance - " + test.Description);

                if (tokens[i].TokenValue is TokenValueOpenParenthesis)
                    Assert.IsTrue(tokens[i].TokenValue == TokenValueOpenParenthesis.Instance, "Test get token: open parenthesis instance - " + test.Description);

                if (tokens[i].TokenValue is TokenValueCloseParenthesis)
                    Assert.IsTrue(tokens[i].TokenValue == TokenValueCloseParenthesis.Instance, "Test get token: close parenthesis instance - " + test.Description);

                // Trim() because the token plus coming with space " + "
                Assert.IsTrue(outputExpectedSplit[i] == tokens[i].TokenValue.ToString().Trim(), "Test get token: compare ouput - " + test.Description);
            }
        }

        #endregion
                
        [TestMethod]
        public void TestMultiplesExpressions()
        {
            var tests = new List<TestCaseDependenceExpression>();

            #region multiples expressions
           
            
            tests.Add(
                new TestCaseDependenceExpression()
                {
                    ProjectMain            = "A",
                    Input                  = " A + B + C + D + (X+Z) + (((E+J) + (K+E+I+P+(X+J+Y))))",
                    OutputExpected         = "A + B + C + D + (X + Z + J + Y) + (E + J + (K + E + I + P + X))",
                    OutputExpectedFullTree = "A + B + C + D + (X + Z + J + Y) + (E + J + (K + E + I + P + (X + Z + J + Y)))",
                    Description            = "Test complex expression, with circular reference in E"
                }
            );
    
            // for now, the builder the builder does not support "_" underline, by limitation of NCalc
            // the builder changes "_" by "."
            tests.Add(
                new TestCaseDependenceExpression()
                {
                    ProjectMain    = "Web",
                    Input          = "Web+DAL.ABC +   (C +    ( J_A+IFF))",
                    OutputExpected = "Web+DAL.ABC +   (C +    ( J.A+IFF))",
                    Description    = "Test change '_' by '.'"
                }
            );

            tests.Add(
                new TestCaseDependenceExpression()
                {
                    ProjectMain = "A",
                    Input       = "A + B + C + D + (E+J) + J",
                    Description = "Test with one parenthesis and withot spaces"
                }
            );

            tests.Add(
                new TestCaseDependenceExpression()
                {
                    ProjectMain = "A",
                    Input       = "A + B + C + D + (E + J) + J",
                    Description = "Test with one parenthesis"
                }
            );

            tests.Add(
                new TestCaseDependenceExpression()
                {
                    ProjectMain            = "A",
                    Input                  = "A + (B + (C + D)) + C",
                    OutputExpectedFullTree = "A + (B + (C + D)) + (C + D)",
                    Description            = "test happy"
                }
            );

            tests.Add(
                new TestCaseDependenceExpression()
                {
                    ProjectMain            = "A",
                    Input                  = "A + (B + (C + D + (A+B))) + C",
                    OutputExpected         = "A + (B + (C + D + A)) + C",
                    OutputExpectedFullTree = "A + (B + (C + D + A)) + C",
                    Description            = "Test recursive, A can't repeat because referenced yourself. C can't be repeat too because referenced A and A reference C causing a recursion"
                }
            );

            tests.Add(
                new TestCaseDependenceExpression()
                {
                    ProjectMain            = "A",
                    Input                  = "A + (B + (C + D + (A+B))) + C + (D+I) + D",
                    OutputExpected         = "A + (B + (C + (D+I) + A)) + C + D",
                    OutputExpectedFullTree = "A + (B + (C + (D+I) + A)) + C + (D+I)",
                    Description            = "Test recursive. Same rules of test above. And 'D' change the place because is defined first in middle expression"
                }
            );

            /*

            tests.Add(
                new TestMultiple()
                {
                    Input = "",
                    OutputExpected = "",
                    Description = ""
                }
            );

            */

            #endregion

            foreach (var test in tests)
            {
                DelProject();
                CreateNewProject(test.Input);
                var app = new ApplicationLoader(pathSln).Load();
                var expReader = new ExpressionReader(app.ProjectCollection.GetProjectByName(test.ProjectMain));
                this.TestCommomCenaries(test, expReader);

                expReader = new ExpressionReader(app.ProjectCollection.GetProjectByName(test.ProjectMain), true);
                this.TestCommomCenaries(test, expReader);
            }
        }

        [TestMethod]
        public void TestBugNCalc()
        {
            var expression = "A + B + C + D + (E+J) + J";
            Expression e = new Expression(expression, EvaluateOptions.NoCache);
            e.EvaluateParameter += delegate(string name, ParameterArgs args)
            {
                args.Result = 1;
            };
            var a = (int)e.Evaluate();
            Assert.IsTrue(a == 7);
        } 

        [TestMethod]
        public void TestParseTokens()
        {
            DelProject();
            var expressionInput = "A + (B + (C + D)) + C";
            CreateNewProject(expressionInput);
            var app = new ApplicationLoader(pathSln).Load();
            var tokens = new ExpressionReader(app.ProjectCollection.GetProjectByName("A")).GetTokens();

            Assert.IsTrue(tokens[0].TokenValue.ToString() == "A");
            Assert.IsTrue(tokens[0].Level == 1);
            Assert.IsTrue(tokens[0].Parent == null);

            Assert.IsTrue(tokens[1].TokenValue.ToString() == " + ");
            Assert.IsTrue(tokens[1].Level == 2);
            Assert.IsTrue(tokens[1].Parent == tokens[0]);
            Assert.IsTrue(tokens[1].TokenValue == TokenValuePlus.Instance);

            Assert.IsTrue(tokens[2].TokenValue.ToString() == "(");
            Assert.IsTrue(tokens[2].Level == 2);
            Assert.IsTrue(tokens[2].Parent == tokens[0]);

            Assert.IsTrue(tokens[3].TokenValue.ToString() == "B");
            Assert.IsTrue(tokens[3].Level == 2);
            Assert.IsTrue(tokens[3].Parent == tokens[0]);

            Assert.IsTrue(tokens[4].TokenValue.ToString() == " + ");
            Assert.IsTrue(tokens[4].Level == 3);
            Assert.IsTrue(tokens[4].Parent == tokens[3]);
            Assert.IsTrue(tokens[4].TokenValue == TokenValuePlus.Instance);

            Assert.IsTrue(tokens[5].TokenValue.ToString() == "(");
            Assert.IsTrue(tokens[5].Level == 3);
            Assert.IsTrue(tokens[5].Parent == tokens[3]);
            Assert.IsTrue(tokens[5].TokenValue == TokenValueOpenParenthesis.Instance);

            Assert.IsTrue(tokens[6].TokenValue.ToString() == "C");
            Assert.IsTrue(tokens[6].Level == 3);
            Assert.IsTrue(tokens[6].Parent == tokens[3]);

            Assert.IsTrue(tokens[7].TokenValue.ToString() == " + ");
            Assert.IsTrue(tokens[7].Level == 4);
            Assert.IsTrue(tokens[7].Parent == tokens[6]);
            Assert.IsTrue(tokens[7].TokenValue == TokenValuePlus.Instance);

            Assert.IsTrue(tokens[8].TokenValue.ToString() == "D");
            Assert.IsTrue(tokens[8].Level == 4);
            Assert.IsTrue(tokens[8].Parent == tokens[6]);

            Assert.IsTrue(tokens[9].TokenValue.ToString() == ")");
            Assert.IsTrue(tokens[9].Level == 3);
            Assert.IsTrue(tokens[9].Parent == tokens[3]);
            Assert.IsTrue(tokens[9].TokenValue == TokenValueCloseParenthesis.Instance);

            Assert.IsTrue(tokens[10].TokenValue.ToString() == ")");
            Assert.IsTrue(tokens[10].Level == 2);
            Assert.IsTrue(tokens[10].Parent == tokens[0]);
            Assert.IsTrue(tokens[10].TokenValue == TokenValueCloseParenthesis.Instance);

            Assert.IsTrue(tokens[11].TokenValue.ToString() == " + ");
            Assert.IsTrue(tokens[11].Level == 2);
            Assert.IsTrue(tokens[11].Parent == tokens[0]);
            Assert.IsTrue(tokens[11].TokenValue == TokenValuePlus.Instance);

            Assert.IsTrue(tokens[12].TokenValue.ToString() == "C");
            Assert.IsTrue(tokens[12].Level == 2);
            Assert.IsTrue(tokens[12].Parent == tokens[0]);
            Assert.IsTrue(tokens[12].TokenValue == tokens[6].TokenValue);
        }
    }
}