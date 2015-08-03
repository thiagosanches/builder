using NCalc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Builder.Dispatcher
{
    public class ExpressionResult
    {
        public Project Project { get; private set; }
        public string Value { get; private set; }
        public string ValueFactored { get; set; }
        internal ExpressionResult(Project project, string valueDefault)
        {
            this.Project = project;
            this.Value = valueDefault;
        }
    }
}
