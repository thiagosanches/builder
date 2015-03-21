using Microsoft.Build.Evaluation;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Builder.Dispatcher
{
    /// <summary>
    /// Summary description for Program.
    /// </summary>
    public class ProjectLoader
    {
        private ProjectCollection ProjectCollection;

        public string Path { get; private set; }
        
        public ProjectLoader(string path)
        {
            this.Path = path;
            this.ProjectCollection = new ProjectCollection();
        }

        public Project LoadProject()
        {
            return this.LoadProject(this.Path, this.Path);
        }

        private Project LoadProject(string projectPath, string parentProjectPath)
        {
            try
            {
                Project project = this.ProjectCollection.GetProjectByPath(projectPath);
                if (project == null)
                {
                    Console.WriteLine("Loading project '{0}'", projectPath);
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(projectPath);

                    XmlNamespaceManager ns = new XmlNamespaceManager(xmldoc.NameTable);
                    ns.AddNamespace("m", "http://schemas.microsoft.com/developer/msbuild/2003");

                    var projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath);
                    var projectGuid = xmldoc.SelectSingleNode(@"//m:PropertyGroup/m:ProjectGuid", ns).InnerText;

                    project = new Project(projectName, projectGuid, this.ProjectCollection);
                    project.Path = projectPath;

                    var projectsDependences = xmldoc.SelectNodes(@"//m:ProjectReference", ns);
                    foreach (XmlNode pRef in projectsDependences)
                    {   
                        var pathRef = PathHelper.MakeAbsolute(parentProjectPath, pRef.Attributes["Include"].InnerText);
                        Console.WriteLine("Loading dependency '{0}' of project '{1}'", pathRef, projectPath);
                        var projectRef = LoadProject(pathRef, pathRef);
                        project.AddReference(projectRef);
                    }
                }

                return project;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Important comment, way to load using Microsoft.Build.Evaluate

        //private Project LoadProject(string projectPath, string relativePath)
        //{
        //    try
        //    {
        //        Project project = this.Context.GetProjectByPath(projectPath);
        //        if (project == null)
        //        {
                   
        //            var collection = new ProjectCollection();
        //            var dotNetProject = collection.LoadProject(projectPath);

        //            var projectName = System.IO.Path.GetFileNameWithoutExtension(dotNetProject.FullPath);
        //            var projectGuid = dotNetProject.Properties.FirstOrDefault(f => f.Name == "ProjectGuid");

        //            project = new Project(projectName, projectGuid.EvaluatedValue, this.Context);
        //            project.Path = projectPath;

        //            var projectsDependences = dotNetProject.AllEvaluatedItems.Where(f => f.ItemType == "ProjectReference").ToList();
        //            foreach (var pRef in projectsDependences)
        //            {
        //                var pathRef = PathHelper.MakeAbsolute(relativePath, pRef.EvaluatedInclude);
        //                var projectRef = LoadProject(pathRef, pathRef);
        //                project.AddReference(projectRef);
        //            }
        //        }

        //        return project;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion
    }
}
