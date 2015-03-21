using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Builder.Template.CSharp.Template;
using Builder.Template;
using Builder.Dispatcher;

namespace Builder.Template.CSharp
{
    public class BuilderCSharp : IBuilder
    {
        public void Build(Application application)
        {
            try
            {
                foreach (var project in application.ProjectCollection.Projects)
                {
                    this.WriteLibrary(application.Path, project);
                }

                this.WriteFile(application.Path, string.Format("{0}.sln", application.Name), this.CreateSolution(application));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void WriteLibrary(string baseDir, Project project)
        {
            string baseDirProject = System.IO.Path.Combine(baseDir, project.Name);
            
            if (!System.IO.Directory.Exists(baseDirProject))
            {
                string propertiesPath = Path.Combine(baseDirProject, "Properties");

                Directory.CreateDirectory(baseDirProject);
                Directory.CreateDirectory(propertiesPath);

                this.WriteFile(baseDirProject, string.Format("{0}.csproj", project.Name), this.CreateCsProj(project));
                this.WriteFile(baseDirProject, @"Properties\AssemblyInfo.cs", this.CreateAssemblyInfo(project));
            }
        }

        private void WriteFile(string baseDirProject, string fileName, string content)
        {
            string fullPath = System.IO.Path.Combine(baseDirProject, fileName);
            System.IO.File.WriteAllText(fullPath, content, Encoding.UTF8);
        }

        #region Create files by template t4

        private string CreateCsProj(Project project)
        {
            var template = new XmlCsProjTemplate();
            template.Session = new Dictionary<string, object>();
            template.Session.Add("__project__", project);
            template.Initialize();
            return template.TransformText();
        }

        private string CreateAssemblyInfo(Project project)
        {
            var template = new AssemblyInfoTemplate();
            template.Session = new Dictionary<string, object>();
            template.Session.Add("_namespaceName", project.Name);
            template.Session.Add("_guid", Guid.NewGuid().ToString());
            template.Session.Add("_year", DateTime.Now.Year);
            template.Session.Add("_version", "1.0.0.0");
            template.Initialize();
            return template.TransformText();
        }

        private string CreateSolution(Application application)
        {
            var template = new SolutionTemplate();
            template.Session = new Dictionary<string, object>();
            template.Session.Add("__app__", application);
            template.Initialize();
            return template.TransformText();
        }

        #endregion
    }
}