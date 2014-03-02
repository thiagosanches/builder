using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Builder.Unit
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Builder.Template.Builder builder = new Template.Builder();
            
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            builder.Build("teste.xml", desktop, "MyTemplateTest");
        }
    }
}