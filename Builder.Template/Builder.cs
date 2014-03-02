using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Builder.Template
{
    public class Builder
    {
        public void Build(string template, string path, string projectName)
        {
            try
            {
                string temporaryDirectory = string.Format(@"{0}\{1}_{2}_{3}", path, DateTime.Now.Hour,
                    DateTime.Now.Minute, DateTime.Now.Second);

                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.PreserveWhitespace = true;
                xml.Load(template);

                ConfigureLibrary(xml, projectName, Layer.Business);
                WriteLibraryProject(xml, temporaryDirectory, projectName, Layer.Business);

                ConfigureLibrary(xml, projectName, Layer.Data);
                WriteLibraryProject(xml, temporaryDirectory, projectName, Layer.Data);

                ConfigureLibrary(xml, projectName, Layer.Model);
                WriteLibraryProject(xml, temporaryDirectory, projectName, Layer.Model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ConfigureLibrary(System.Xml.XmlDocument xml, string projectName, Layer layer)
        {
            System.Xml.XmlNodeList projectGuid = xml.GetElementsByTagName("ProjectGuid");
            projectGuid[0].InnerXml = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

            System.Xml.XmlNodeList rootNamespace = xml.GetElementsByTagName("RootNamespace");
            rootNamespace[0].InnerXml = string.Format("{0}.{1}", projectName, layer);

            System.Xml.XmlNodeList assemblyName = xml.GetElementsByTagName("AssemblyName");
            assemblyName[0].InnerXml = string.Format("{0}.{1}", projectName, layer);
        }

        private void WriteLibraryProject(System.Xml.XmlDocument xml, string baseDir, string projectName, Layer layer)
        {
            string path = System.IO.Path.Combine(baseDir, projectName + "." + layer.ToString());

            if (!System.IO.Directory.Exists(path))
            {
                string propertiesPath = Path.Combine(path, "Properties");

                Directory.CreateDirectory(path);
                Directory.CreateDirectory(propertiesPath);

                string fileName = string.Format("{0}.{1}.csproj", projectName, layer);
                string fullPath = System.IO.Path.Combine(path, fileName);

                System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(fullPath, Encoding.UTF8);
                xml.WriteTo(writer);
                writer.Flush();
                writer.Close();

                string fullAssemblyInfo = propertiesPath + "\\AssemblyInfo.cs";
                File.WriteAllText(fullAssemblyInfo, CreateAssemblyInfo(projectName, layer), Encoding.UTF8);
            }
        }

        private string CreateAssemblyInfo(string projectName, Layer layer)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System.Reflection;");
            stringBuilder.AppendLine("using System.Runtime.CompilerServices;");
            stringBuilder.AppendLine("using System.Runtime.InteropServices;");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("// General Information about an assembly is controlled through the following ");
            stringBuilder.AppendLine("// set of attributes. Change these attribute values to modify the information");
            stringBuilder.AppendLine("// associated with an assembly.");

            stringBuilder.AppendFormat("[assembly: AssemblyTitle(\"{0}.{1}\")]", projectName, layer);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("[assembly: AssemblyDescription(\"\")]");
            stringBuilder.AppendLine("[assembly: AssemblyConfiguration(\"\")]");
            stringBuilder.AppendLine("[assembly: AssemblyCompany(\"\")]");
            stringBuilder.AppendFormat("[assembly: AssemblyProduct(\"{0}.{1}\")]", projectName, layer);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("[assembly: AssemblyCopyright(\"Copyright ©  {0}\")]", DateTime.Now.Year);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("[assembly: AssemblyTrademark(\"\")]");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("[assembly: AssemblyCulture(\"\")]");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("// Setting ComVisible to false makes the types in this assembly not visible ");
            stringBuilder.AppendLine("// to COM components.  If you need to access a type in this assembly from ");
            stringBuilder.AppendLine("// COM, set the ComVisible attribute to true on that type.");
            stringBuilder.AppendLine("[assembly: ComVisible(false)]");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("// The following GUID is for the ID of the typelib if this project is exposed to COM");
            stringBuilder.AppendFormat("[assembly: Guid(\"{0}\")]", Guid.NewGuid());
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("// Version information for an assembly consists of the following four values:");
            stringBuilder.AppendLine("//");
            stringBuilder.AppendLine("//      Major Version");
            stringBuilder.AppendLine("//      Minor Version ");
            stringBuilder.AppendLine("//      Build Number");
            stringBuilder.AppendLine("//      Revision");
            stringBuilder.AppendLine("//");
            stringBuilder.AppendLine("// You can specify all the values or you can default the Build and Revision Numbers ");
            stringBuilder.AppendLine("// by using the '*' as shown below:");
            stringBuilder.AppendLine("// [assembly: AssemblyVersion(\"1.0.*\")]");
            stringBuilder.AppendLine("[assembly: AssemblyVersion(\"1.0.0.0\")]");
            stringBuilder.AppendLine("[assembly: AssemblyFileVersion(\"1.0.0.0\")]");

            return stringBuilder.ToString();
        }

        public enum Layer
        {
            Business,
            Data,
            Model,
            Web
        }
    }
}