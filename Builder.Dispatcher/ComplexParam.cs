using NCalc;
using System;
using System.Collections.Generic;

namespace Builder.Dispatcher
{
    internal class ProjectParamExpression : IOperations
    {
        private Project project;
        public ProjectParamExpression(Project project)
        {
            this.project = project;
        }

        #region Implements IOperations

        object IOperations.Add(object b)
        {
            var p = (ProjectParamExpression)b;
            this.project.AddReference(p.project);
            return this;
        }

        object IOperations.Soustract(object b)
        {
            var p = (ProjectParamExpression)b;
            this.project.RemoveReference(p.project);
            return this;
        }

        object IOperations.Multiply(object b)
        {
            throw new NotImplementedException();
        }

        object IOperations.Divide(object b)
        {
            throw new NotImplementedException();
        }

        object IOperations.Modulo(object b)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
