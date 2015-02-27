using Builder.Template.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Builder.Core
{
    public static class BuilderFactory
    {
        public static IBuilder GetFactory(string templateLibraryName) 
        {
            IBuilder builder = FactoryHelper.GetInstance<IBuilder>(templateLibraryName);
            return builder;
        }
    }
}
