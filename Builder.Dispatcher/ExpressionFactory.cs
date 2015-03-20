using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Dispatcher
{
    /// <summary>
    /// Summary description for Program.
    /// </summary>
    public static class ExpressionFactory
    {
        public static void LoadApplicationByExpression(Application application, string expression)
        {
            //Expression e = new Expression("Web+(Business+Dal+(D+C))", EvaluateOptions.None);
            
            // FIX to resolve params with name contains ".", ex: "Namespace.ProjectName"
            expression = expression.Replace('.', '_');

            Expression e = new Expression(expression);

            e.EvaluateFunction += delegate(string name, FunctionArgs args)
            {
                if (name == "TESTE")
                {
                    var value = args.Parameters[0].Evaluate();
                    args.Result = value;
                }
            };

            e.EvaluateParameter += delegate(string name, ParameterArgs args)
            {
                // FIX to back params name
                name = name.Replace('_', '.');
                var projectAdd = application.Projects.FirstOrDefault(f => f.Name == name);
                if (projectAdd == null)
                {
                    projectAdd = new Project(application, name, Guid.NewGuid().ToString().ToUpper());
                    application.Projects.Add(projectAdd);
                }

                var param = new ProjectParamExpression(projectAdd);
                args.Result = param;
            };

            e.Evaluate();
        }
    }
}
