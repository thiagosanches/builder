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
        private string path;
        private bool isLoaded;

        public ProjectLoader(string path, ProjectCollection collection = null)
        {
            this.path = path;
            if (collection == null)
                collection = new ProjectCollection();

            this.ProjectCollection = collection;
        }

        public Project Load()
        {
            if (isLoaded)
                throw new Exception("Project loader discarted because already used");

            isLoaded = true;

            return this.Load(this.path, this.path);
        }

        private Project Load(string projectPath, string parentProjectPath)
        {
            try
            {
                Project project = this.ProjectCollection.GetProjectByPath(projectPath);
                if (project == null)
                {
                    var projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath);
                    Console.WriteLine("Loading project '{0}'", projectName);

                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(projectPath);

                    XmlNamespaceManager ns = new XmlNamespaceManager(xmldoc.NameTable);
                    ns.AddNamespace("m", "http://schemas.microsoft.com/developer/msbuild/2003");

                    var projectGuid = xmldoc.SelectSingleNode(@"//m:PropertyGroup/m:ProjectGuid", ns).InnerText;

                    var exists = this.ProjectCollection.Projects.FirstOrDefault(f => f.Name.ToLower() == projectName.ToLower() || f.Path.ToLower() == projectPath.ToLower());
                    if (exists != null)
                    {
                        project = exists;
                    }
                    else 
                    { 
                        project = new Project(projectName, projectGuid, this.ProjectCollection, projectPath);
                        project.Path = projectPath;
                    }

                    var projectsReference = xmldoc.SelectNodes(@"//m:Reference", ns);
                    foreach (XmlNode pRef in projectsReference)
                    {
                        var pathRef = "";
                        var nodeHintPath = GetChildByTagName(pRef, "HintPath");
                        if (nodeHintPath != null)
                            pathRef = PathHelper.MakeAbsolute(parentProjectPath, pRef.ChildNodes[0].InnerText);                        
                        
                        var pRefName = pRef.Attributes["Include"].InnerText;

                        Project projectAdd;
                        exists = this.ProjectCollection.Projects.FirstOrDefault(f => f.Name.ToLower() == pRefName.ToLower() || f.Path.ToLower() == pathRef.ToLower());
                        if (exists != null)
                        {
                            projectAdd = exists;
                        }
                        else 
                        { 
                            projectAdd = new Project(pRefName, null, this.ProjectCollection, pathRef);
                        }

                        Console.WriteLine("Loading DLL dependency '{0}' of project '{1}'", pRefName, projectName);
                        project.AddReference(projectAdd);
                    }

                    var projectsDependences = xmldoc.SelectNodes(@"//m:ProjectReference", ns);
                    foreach (XmlNode pRef in projectsDependences)
                    {   
                        var pathRef = PathHelper.MakeAbsolute(parentProjectPath, pRef.Attributes["Include"].InnerText);
                        var pRefName = System.IO.Path.GetFileNameWithoutExtension(pathRef);

                        Console.WriteLine("Loading dependency '{0}' of project '{1}'", pRefName, projectName);
                        var projectRef = Load(pathRef, pathRef);
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

        public XmlNode GetChildByTagName(XmlNode nodeParent, string tagName)
        {
            foreach (XmlNode item in nodeParent.ChildNodes)
                if (item.Name == tagName)
                    return item;

            return null;
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
