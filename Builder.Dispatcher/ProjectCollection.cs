using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Builder.Dispatcher
{
    public class ProjectCollection
    {
        private List<Project> projects;

        public IEnumerable<Project> Projects
        {
            get 
            {
                return (IEnumerable<Project>)projects;
            }
        }

        public ProjectCollection()
        {
            this.projects = new List<Project>();
        }

        public Project GetProjectByName(string name)
        {
            return this.projects.FirstOrDefault(f => f.Name == name);
        }

        public Project GetProjectByPath(string path)
        {
            return this.projects.FirstOrDefault(f => f.Path == path);
        }

        public Project AddProject(Project project)
        {
            var fixDifferentPathAndSameName = true;
            var fixDifferentNameAndSamePath = true;

            var exists = this.projects.FirstOrDefault(f => f == project || f.Name.ToLower() == project.Name.ToLower() || f.Path.ToLower() == project.Path.ToLower());
            if (exists == null)
            {
                this.projects.Add(project);               
            }
            else if (exists != null
                && exists.Path != ""
                && project.Path != ""
                && exists.Name.ToLower() == project.Name.ToLower() 
                && exists.Path.ToLower() != project.Path.ToLower())
            {
                var details = "Added name: " + exists.Name;
                details += "\r\nAdded path: " + exists.Path;
                details += "\r\nTry add name: " + project.Name;
                details += "\r\nTry add path: " + project.Path;

                var msg = string.Format("Project '{0}' alredy exists in project collection with same Name and different Path\r\n{1}", project.Name, details);

                if (!fixDifferentPathAndSameName)
                {
                    throw new Exception(msg);
                }
                else 
                {
                    //exists.Name = exists.Name + " (" + exists.Path + ")";
                    //project.Name = project.Name + " (" + project.Path + ")";

                    var before = System.Console.ForegroundColor;
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.Write(msg);
                    System.Console.ForegroundColor = before;

                    //this.projects.Add(project);
                    return exists;
                }
            }
            else if (exists != null
                && exists.Path != ""
                && project.Path != ""
                && exists.Name.ToLower() != project.Name.ToLower()
                && exists.Path.ToLower() == project.Path.ToLower())
            {
                var details = "Added name: " + exists.Name;
                details += "\r\nAdded path: " + exists.Path;
                details += "\r\nTry add name: " + project.Name;
                details += "\r\nTry add path: " + project.Path;

                var msg = string.Format("Project '{0}' alredy exists in project collection with same Path and different Name\r\n{1}", project.Name, details);
                if (!fixDifferentNameAndSamePath)
                {
                    throw new Exception(msg);
                }
                else
                {
                    var before = System.Console.ForegroundColor;
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.Write(msg);
                    System.Console.ForegroundColor = before;
                    return exists;
                }
            }

            return project;
        }

        public void RemoveProject(Project project)
        {
            this.projects.Remove(project);
            this.projects.RemoveAll(f => f.Name == project.Name);
        }

        #region Expression methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        public void ApplyExpression(string expression)
        {
            // FIX to resolve params with name contains ".", ex: "Namespace.ProjectName"
            expression = expression.Replace('.', '_');

            // FIX to resolve the stranged problema with expression: A + B + C + D + (E+J) + J
            // the expression "(E+J)" dosent parsed, the "E" params is ignored
            //expression = expression.Replace(" ", "");
            //expression = expression.Replace("+", " + ");
            //expression = expression.Replace("-", " - ");

            Expression e = new Expression(expression, EvaluateOptions.NoCache);

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
                var projectAdd = this.GetProjectByName(name);
                if (projectAdd == null)
                {
                    projectAdd = new Project(name, System.Guid.NewGuid().ToString().ToUpper());
                    this.AddProject(projectAdd);
                }

                var param = new ProjectParamExpression(projectAdd);
                args.Result = param;
            };

            e.Evaluate();
        }

        #endregion

        #region ToString Methods

        public Dictionary<string, string> ToExpression(bool showFullTree)
        {
            var dictionaries = new Dictionary<string, string>();
            foreach (var project in this.projects)
                dictionaries.Add(project.Name, project.ToStringExpression(showFullTree));

            return dictionaries;
        }

        public Dictionary<string, string> ToStringHierarchy(bool showFullTree)
        {
            var dictionaries = new Dictionary<string, string>();
            foreach (var project in this.projects)
                dictionaries.Add(project.Name, project.ToStringHierarchy(showFullTree));

            return dictionaries;
        }

        public Dictionary<string, string> ToStringHierarchyInverse()
        {
            var dictionaries = new Dictionary<string, string>();
            foreach (var project in this.projects)
                dictionaries.Add(project.Name, project.ToStringHierarchyInverse());

            return dictionaries;
        }

        #endregion

    }
}
