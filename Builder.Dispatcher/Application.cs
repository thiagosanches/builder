using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Builder.Dispatcher
{
    public class Application
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Guid { get; private set; }
        public ProjectCollection ProjectCollection { get; private set; }

        public Application(string name, string path, string guid)
            : this(name, path, guid, new ProjectCollection())
        {
        }

        public Application(string name, string path, string guid, ProjectCollection projectCollection)
        {
            if (projectCollection == null)
                new Exception("Project collection can't be null in application instance");

            this.Name = name;
            this.Path = path;
            this.Guid = "{" + guid.ToUpper() + "}";
            this.ProjectCollection = projectCollection;
        }

        /// <summary>
        /// Remover este método, pq esse projeto não deveria conhecer a extensão do arquivo
        /// Adicionado temporariamente 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public string GetRelativePathToSolution(Project project)
        {
            return project.Name + "/" + project.Name + ".csproj";
        }
    }
}
