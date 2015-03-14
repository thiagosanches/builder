using System;
namespace Builder.Template.Interface
{
    public interface IBuilder
    {
        void Build(string baseDir, string projectName);
    }
}
