using NCalc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Builder.Dispatcher
{
    public class Project
    {

        public string Name { get; internal set; }
        public List<Project> ProjectsReferences { get; private set; }
        public List<Project> ProjectsParents { get; private set; }
        public string Path { get; set; }
        
        private string guid;
        public string Guid
        {
            get
            {
                return guid;
            }
            private set
            {
                if (value != null)
                { 
                    guid = value;
                    if (guid[0] != '{')
                        guid = "{" + guid + "}";
                }
            }
        }

        /// <summary>
        /// Is a external bag, similar to solucition in c#, but with as unique responsible the to be a collection
        /// </summary>
        public ProjectCollection ProjectCollection { get; private set; }

        public Project(string name, string guid, ProjectCollection projectCollection = null, string projectPath = null)
        {
            this.Name = name;
            this.ProjectsReferences = new List<Project>();
            this.ProjectsParents = new List<Project>();
            this.Guid = guid == null ? null : guid.ToUpper();
            this.ProjectCollection = projectCollection;
            this.Path = projectPath;
            
            if (this.ProjectCollection != null)
                this.ProjectCollection.AddProject(this);
        }

        public void AddReference(Project project)
        {
            var exists = this.ProjectsReferences.Exists(f => f == project);
            if (!exists)
            {
                var existsRef = this.ProjectsReferences.FirstOrDefault(f => f.Name == project.Name);
                if (existsRef != null && existsRef.Path.ToLower() != project.Path.ToLower())
                {
                    //existsRef.Name = existsRef.Name + " (" + existsRef.Path + ")";
                    //project.Name = project.Name + " (" + project.Path + ")";

                    throw new Exception(string.Format("Project '{0}' alredy exists in project {1}", project.Name, this.Name));
                }

                this.ProjectsReferences.Add(project);
                project.ProjectsParents.Add(this);
                if (this.ProjectCollection != null)
                    this.ProjectCollection.AddProject(project);
            }
        }

        public bool HasDirectOrIndirectReference(Project projectVerify)
        {
            foreach(var p in this.ProjectsReferences)
            {
                if (p == projectVerify)
                    return true;

                if (p.HasDirectOrIndirectReference(projectVerify))
                    return true;
            }

            return false;
        }

        public void RemoveReference(Project project)
        {
            var countRem = this.ProjectsReferences.RemoveAll(f => f.Name == project.Name);
            
            if (countRem > 0) 
            { 
                project.ProjectsParents.Remove(this);
                project.ProjectsParents.RemoveAll(f => f.Name == this.Name);
            }

            if (this.ProjectCollection != null) 
                this.ProjectCollection.RemoveProject(project);
        }

        #region ToString Methods

        public string ToStringExpression(bool showFullTree = false)
        {
            var strBuilder = new StringBuilder();
            var loadeds = new Dictionary<string, string>();

            strBuilder.Append(ToStringExpression(this, showFullTree, loadeds));

            //strBuilder.AppendLine("--------");
            //strBuilder.AppendLine("Total projects: " + loadeds.Count);
            //strBuilder.AppendLine("Total projects 2: " + this.ProjectCollection.Projects.Count());

            //strBuilder.AppendLine("DEBUG");
            //foreach (var i in loadeds)
            //    strBuilder.AppendLine(i.Key);

            //strBuilder.AppendLine("bag:DEBUG");
            //foreach (var i in this.ProjectCollection.Projects)
            //    strBuilder.AppendLine(i.Name);

            return strBuilder.ToString();
        }

        private string ToStringExpression(Project project, bool showFullTree, Dictionary<string, string> loadeds)
        {
            var exp = project.Name;
            if (project.ProjectsReferences.Count > 0)
            {
                foreach (var pRef in project.ProjectsReferences)
                {
                    exp += " + ";

                    if (loadeds.ContainsKey(pRef.Name))
                    {
                        if (showFullTree)
                        {
                            if (pRef.ProjectsReferences.Count == 0)
                                exp += loadeds[pRef.Name];
                            else
                                exp += "(" + loadeds[pRef.Name] + ")";
                        }
                        else
                            exp += pRef.Name;
                    }
                    else
                    {
                        if (pRef.ProjectsReferences.Count == 0)
                            exp += ToStringExpression(pRef, showFullTree, loadeds);
                        else
                            exp += "(" + ToStringExpression(pRef, showFullTree, loadeds) + ")";
                    }
                }
            }

            if (!loadeds.ContainsKey(project.Name))
                loadeds.Add(project.Name, exp);

            return exp;
        }

        public string ToStringHierarchy(bool showFullTree = false)
        {
            var strBuilder = new StringBuilder();
            var loadeds = new List<string>();
            ToStringHierarchy(this, strBuilder, showFullTree, loadeds);
            strBuilder.AppendLine();

            //strBuilder.AppendLine("--------");
            //strBuilder.AppendLine("Total projects: " + loadeds.Count);
            //strBuilder.AppendLine("Total projects 2: " + this.ProjectCollection.Projects.Count());
            
            //strBuilder.AppendLine("DEBUG");
            //foreach (var i in loadeds)
            //    strBuilder.AppendLine(i);

            //strBuilder.AppendLine("bag:DEBUG");
            //foreach (var i in this.ProjectCollection.Projects)
            //    strBuilder.AppendLine(i.Name);

            return strBuilder.ToString();
        }

        private void ToStringHierarchy(Project project, StringBuilder strBuider, bool showFullTree, List<string> loadeds, int level = 0)
        {
            level++;
            var resume = string.Format(" (count refs: {0}, count inverse refs: {1})", project.ProjectsReferences.Count, project.ProjectsParents.Count);
            strBuider.Append(project.Name + resume);

            if (project.ProjectsReferences.Count > 0)
            {
                foreach (var pRef in project.ProjectsReferences)
                {
                    strBuider.AppendLine();
                    strBuider.Append(new String('.', level * 3));
                    if (!showFullTree && loadeds.Contains(pRef.Name))
                        strBuider.Append(pRef.Name + "*");
                    else 
                    { 
                        ToStringHierarchy(pRef, strBuider, showFullTree, loadeds, level);
                        if (!showFullTree && !loadeds.Contains(pRef.Name))
                            loadeds.Add(pRef.Name);
                    }
                }
            }

            if (!loadeds.Contains(project.Name))
                loadeds.Add(project.Name);
        }

        public string ToStringHierarchyInverse()
        {
            var strBuilder = new StringBuilder();
            var loadeds = new List<string>();
            ToStringHierarchyInverse(this, strBuilder, loadeds);

            //strBuilder.AppendLine("--------");
            //strBuilder.AppendLine("Total projects: " + loadeds.Count);
            //strBuilder.AppendLine("Total projects 2: " + this.ProjectCollection.Projects.Count());

            //strBuilder.AppendLine("cache:DEBUG");
            //foreach (var i in loadeds)
            //    strBuilder.AppendLine(i);

            //strBuilder.AppendLine("bag:DEBUG");
            //foreach (var i in this.ProjectCollection.Projects)
            //    strBuilder.AppendLine(i.Name);

            return strBuilder.ToString();
        }

        private void ToStringHierarchyInverse(Project project, StringBuilder strBuider, List<string> loadeds)
        {
            if (loadeds.Contains(project.Name))
                return;

            //strBuider.AppendLine();
            if (project.ProjectsParents.Count > 0)
            { 
                foreach (Project parent in project.ProjectsParents)
                    strBuider.AppendLine(parent.Name);
            }
            else
            {
                strBuider.AppendLine("[any project references this project]");
            }

            strBuider.AppendLine("..." + project.Name);
            strBuider.AppendLine();
            //strBuider.AppendLine("------------------------");

            foreach (Project pRef in project.ProjectsReferences)
                ToStringHierarchyInverse(pRef, strBuider, loadeds);

            loadeds.Add(project.Name);
        }

        #endregion

        /// <summary>
        /// Remover este método, pq esse projeto não deveria conhecer a extensão do arquivo
        /// Adicionado temporariamente 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public string GetRelativePathToProjectRef(Project project)
        {
            return "../" + project.Name + "/" + project.Name + ".csproj";
        }
    }
}
