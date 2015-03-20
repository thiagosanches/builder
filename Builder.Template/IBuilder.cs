using Builder.Dispatcher;
using System;

namespace Builder.Template
{
    public interface IBuilder
    {
        void Build(Application app);
    }
}
