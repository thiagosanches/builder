using Microsoft.Build.Evaluation;
using NCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Builder.Dispatcher
{
    public class ApplicationLoader
    {
        private string path { get; set; }
        private bool isLoaded;

        public ApplicationLoader(string path)
        {
            this.path = path;
        }

        public Application Load()
        {
            if (isLoaded)
                throw new Exception("Project loader discarted because already used");

            isLoaded = true;

            Application app = null;

            var content = System.IO.File.ReadAllText(path);
            Regex projReg = new Regex(
                "Project\\(\"(\\{[\\w-]*\\})\"\\) = \"([\\w _]*.*)\", \"(.*\\.(cs|vcx|vb)proj)\""
                , RegexOptions.Compiled);
            var matches = projReg.Matches(content).Cast<Match>().ToList();

            if (matches.Count > 0)
            {
                var appName = System.IO.Path.GetFileNameWithoutExtension(path);
                var appGuid = matches[0].Groups[1].Value;

                app = new Application(appName, path, appGuid);

                var projects = matches.Select(x => x.Groups[3].Value).ToList();
                for (int i = 0; i < projects.Count; ++i)
                {
                    var projectPath = PathHelper.MakeAbsolute(this.path, projects[i]);
                    var project = new ProjectLoader(projectPath, app.ProjectCollection).Load();
                }

            }

            return app;
        }
    }
}
