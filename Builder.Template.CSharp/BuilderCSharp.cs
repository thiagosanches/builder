using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Builder.Template.Interface;
using Builder.Template.CSharp.Template;

namespace Builder.Template.CSharp
{
    public class BuilderCSharp : IBuilder
    {
        public enum Layer
        {
            Business,
            Data,
            Model,
            Web
        }

        public void Build(string baseDir, string projectName)
        {
            try
            {
                this.WriteLibrary(baseDir, projectName, Layer.Business);
                this.WriteLibrary(baseDir, projectName, Layer.Model);
                this.WriteLibrary(baseDir, projectName, Layer.Data);
                this.WriteLibrary(baseDir, projectName, Layer.Web);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string WriteLibrary(string baseDir, string projectName, Layer layer)
        {
            string baseDirProject = System.IO.Path.Combine(baseDir, projectName + "." + layer.ToString());

            if (!System.IO.Directory.Exists(baseDirProject))
            {
                string propertiesPath = Path.Combine(baseDirProject, "Properties");

                Directory.CreateDirectory(baseDirProject);
                Directory.CreateDirectory(propertiesPath);

                this.WriteFile(baseDirProject, string.Format("{0}.{1}.csproj", projectName, layer), this.CreateCsProj(projectName, layer));
                this.WriteFile(baseDirProject, string.Format(@"Properties\AssemblyInfo.cs", projectName, layer), this.CreateAssemblyInfo(projectName, layer));
            }

            return baseDirProject;
        }

        private void WriteFile(string baseDirProject, string fileName, string content)
        {
            string fullPath = System.IO.Path.Combine(baseDirProject, fileName);
            File.WriteAllText(fullPath, content, Encoding.UTF8);
        }

        #region Create files by template t4

        private string CreateCsProj(string projectName, Layer layer)
        {
            var template = new XmlCsProjTemplate();
            template.Session = new Dictionary<string, object>();
            template.Session.Add("_projectGuid", "{" + Guid.NewGuid().ToString().ToUpper() + "}");
            template.Session.Add("_rootNamespace", string.Format("{0}.{1}", projectName, layer));
            template.Session.Add("_assemblyName", string.Format("{0}.{1}", projectName, layer));
            template.Initialize();
            return template.TransformText();
        }

        private string CreateAssemblyInfo(string projectName, Layer layer)
        {
            var template = new AssemblyInfoTemplate();
            template.Session = new Dictionary<string, object>();
            template.Session.Add("_namespaceName", projectName + layer);
            template.Session.Add("_guid", Guid.NewGuid().ToString());
            template.Session.Add("_year", DateTime.Now.Year);
            template.Session.Add("_version", "1.0.0.0");
            template.Initialize();
            return template.TransformText();
        }

        #endregion
    }
}