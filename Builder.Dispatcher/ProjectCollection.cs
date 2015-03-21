using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public void AddProject(Project project)
        {
            var exists = this.projects.Exists(f => f == project);
            if (!exists)
            {
                if (this.projects.Exists(f => f.Name == project.Name))
                    throw new Exception(string.Format("Project '{0}' alredy exists in project collection {1}", project.Name));

                this.projects.Add(project);
            }
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

        public Dictionary<string, string> ToExpression(Application application)
        {
            var dictionaries = new Dictionary<string, string>();
            foreach (var project in this.projects)
                dictionaries.Add(project.Name, project.ToStringExpression());

            return dictionaries;
        }

        public Dictionary<string, string> ToStringHierarchy()
        {
            var dictionaries = new Dictionary<string, string>();
            foreach (var project in this.projects)
                dictionaries.Add(project.Name, project.ToStringHierarchy());

            return dictionaries;
        }

        #endregion

    }
}
