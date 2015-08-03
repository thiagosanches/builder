using NCalc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace Builder.Dispatcher
{
    public class ExpressionReader3
    {
        public class Token
        {
            private List<Token> parents = new List<Token>();
            private List<Token> childrens = new List<Token>();
            private Token parent;

            public int Level { get; private set; }
            public bool IsDisplayed { get; set; }
            public object Value { get; private set; }

            public Token Parent 
            {
                get { return parent; }
                set
                {
                    parent = value;
                    if (parent != null)
                    {
                        this.parent.childrens.Add(this);
                        this.parents.Add(parent);
                    }
                }
            }

            public Token(object value, Token parent, int level)
            {
                this.Value = value;
                this.Level = level;
                this.Parent = parent;
            }

            /// <summary>
            /// Vefiry if all the children were displayed
            /// </summary>
            /// <param name="tokens"></param>
            /// <returns></returns>
            public bool AllChildrenDisplayed(List<Token> tokens)
            {
                var childrens = tokens.Where(f => f.Parent == this).ToList();
                if (childrens.Count > 0)
                    return childrens.All(f => f.IsDisplayed);
                else
                    return false;
            }

            public override string ToString()
            {
                var res = "";
                if (this is TokenPlus)
                    res = " + ";
                else if (this is TokenOpenParenthesis)
                    res = "(";
                else if (this is TokenCloseParenthesis)
                    res = ")";
                else if (this is TokenRecursive)
                    res = ((Project)this.Value).Name;
                else
                    res = ((Project)this.Value).Name;

                this.IsDisplayed = true;
                return res;
            }
        }

        public class TokenRecursive : Token
        {
            //public static TokenPlus Instance = new TokenPlus();
            //private TokenPlus() : base("+") { }
            public TokenRecursive(object value, Token parent, int level) : base(value, parent, level) { }
        }

        public class TokenPlus : Token
        {
            //public static TokenPlus Instance = new TokenPlus();
            //private TokenPlus() : base("+") { }
            public TokenPlus(Token parent, int level) : base(null, parent, level) { }
        }

        public class TokenOpenParenthesis : Token
        {
            //public static TokenOpenParenthesis Instance = new TokenOpenParenthesis();
            //public TokenOpenParenthesis() : base("(") { }
            public TokenOpenParenthesis(Token parent, int level) : base(null, parent, level) { }
        }

        public class TokenCloseParenthesis : Token
        {
            //public static TokenCloseParenthesis Instance = new TokenCloseParenthesis();
            //private TokenCloseParenthesis() : base(")") { }
            public TokenCloseParenthesis(Token parent, int level) : base(null, parent, level) { }
        }

        public class Cache
        {
            public List<object> Items { get; set; }

            public Cache(object firstItem = null)
            {
                this.Items = new List<object>();

                if (firstItem != null)
                    this.Items.Add(firstItem);
            }

            private Dictionary<object, string> printeds = new Dictionary<object, string>();
            public override string ToString()
            {
                var ret = "";
                foreach (var item in Items)
                {
                    //if (item is Cache)
                    //    ret += item.GetHashCode().ToString();
                    //else
                        ret += item.ToString();
                }

                return ret;
            }
        }
       /* 
        public class ExpressionPrinter
        {
            public Cache value;
            private Dictionary<object, string> printeds = new Dictionary<object, string>();

            public ExpressionPrinter(Cache value)
            {
                this.value = value;
            }

            public string ToStringRecursive()
            {
                var ret = "";
                foreach (var item in this.value)
                {
                    if (!printeds.ContainsKey(item))
                        printeds.Add(item, null);
                    else
                        printeds[item].ToString();

                    ret += item.ToString();
                }

                return ret;
            }

        }
        * */

        public class Events
        {
            public Func<Project, Cache> InitLeft { get; set; }
            public Func<Project, Cache> PlusRight { get; set; }
            public Func<Project, Cache> InitRightWhenSubExpression { get; set; }
            public Func<Project, Cache> EndRightWhenSubExpression { get; set; }
        }

        private ProjectCollection projectCollection;
        private Project project;
        private bool showFullTree;
        private Dictionary<Project, Cache> cacheParsers = new Dictionary<Project, Cache>();
        private List<Token> tokens = new List<Token>();

        public ExpressionReader3(ProjectCollection collection, bool showFullTree = false)
        {
            this.projectCollection = collection;
            this.showFullTree = showFullTree;
        }

        public ExpressionReader3(Project project, bool showFullTree = false)
        {
            this.project = project;
            this.showFullTree = showFullTree;
        }

        public List<ExpressionResult> Read()
        {
            var returnExpResult = new List<ExpressionResult>();
            var currentParsedExpression = new Dictionary<Project,string>();

            Action<Project, string> funcValueDefault = delegate(Project p, string e) 
            {
                if (!returnExpResult.Exists(f=>f.Project == p))
                    returnExpResult.Add(new ExpressionResult(p, e));
            };

            Action<Project, string> funcValueFactored = delegate(Project p, string e)
            {
                var resultValue = returnExpResult.FirstOrDefault(f=>f.Project == p);
                resultValue.ValueFactored = e;
            };

            foreach (var project in this.projectCollection.Projects)
            {
                ToExpression(project, currentParsedExpression, funcValueDefault);
            }

            foreach (var project in this.projectCollection.Projects)
            {
                ToExpression(project, currentParsedExpression, funcValueFactored);
            }

            //if (alreadyProcessedExpressions != null)
            //{
            //    var reverse = new Dictionary<Project, string>();
            //    for (var i = returnExpResult.Count - 1; i >= 0; i--)
            //        reverse.Add(returnExpressions.Keys.ElementAt(i), returnExpressions.Values.ElementAt(i));
            //    return reverse;
            //}

            return returnExpResult;
        }

        public Dictionary<Project, string> Read(Project project)
        {
            //var returnExpResult = new List<ExpressionResult>();
            //var currentParsedExpression = new Dictionary<Project, string>();
            //var returnExpressions = new Dictionary<Project, string>();
            //ToExpression(project, currentParsedExpression, returnExpResult);
            //return returnExpressions;
            return null;
        }

        public string Read2()
        {
            var returnExpResult = new List<ExpressionResult>();
            StringBuilder strBuilder = new StringBuilder();

            var events = new Events();
            events.InitLeft = (left) => new Cache(left.Name);
            events.PlusRight = (right) => new Cache(" + ");
            events.InitRightWhenSubExpression = (right) => new Cache(" + (");
            events.EndRightWhenSubExpression = (right) => new Cache(")");

            //this.Recursive(this.project, events);
            this.Recursive2(null, this.project);

            var res = "";
            foreach(var token in tokens)
            {
                if (token is TokenPlus)
                    res += " + ";
                else if (token is TokenOpenParenthesis)
                    res += "(";
                else if (token is TokenCloseParenthesis)
                    res += ")";
                else if (token is TokenRecursive)
                    res += ((Project)token.Value).Name;
                else
                    res += ((Project)token.Value).Name;
            }

            var res2 = "";
            foreach (var token in tokens)
            {
                //if (token.IsDisplayed)
                //    continue;

                var resParent = "";
                if (token.Parent != null) 
                    resParent = string.Format(" parent: ({0} - {1})", + token.Parent.GetHashCode(), token.Parent.ToString());

                var level = " level: " + token.Level;

                res2 += token.ToString().Trim() + " " + token.GetHashCode() + resParent + level + "\r\n";
            }
            return strBuilder.ToString();
        }

        private Token AddTokenOperand<T>(object parentValue, int level) where T : Token
        {
            var existsTokenParent = tokens.FirstOrDefault(f => f.Value == parentValue);
            if (typeof(T) == typeof(TokenPlus))
                return new TokenPlus(existsTokenParent, level);
            else if (typeof(T) == typeof(TokenOpenParenthesis))
                return new TokenOpenParenthesis(existsTokenParent, level);
            else if (typeof(T) == typeof(TokenCloseParenthesis))
                return new TokenCloseParenthesis(existsTokenParent, level);

            return null;
        }

        private Token GetOrCreateTokenProject<T>(Project parent, Project current, int level) where T : Token
        {
            var existsTokenCurrent = tokens.FirstOrDefault(f => f.Value == current);
            if (existsTokenCurrent != null)
                return existsTokenCurrent;

            var existsTokenParent = tokens.FirstOrDefault(f => f.Value == parent);

            if (typeof(T) == typeof(TokenRecursive))
                return new TokenRecursive(current, existsTokenParent, level);
            else
                return new Token(current, existsTokenParent, level);
        }

        private void Recursive2(Project parent, Project current, int level = 0)
        {
            level++;
            tokens.Add(GetOrCreateTokenProject<Token>(parent, current, level));

            if (current.ProjectsReferences.Count > 0)
            {
                foreach (var next in current.ProjectsReferences)
                {
                    var exists = tokens.FirstOrDefault(f => f.Value == next);
                    if (exists != null && !showFullTree)
                    {
                        tokens.Add(AddTokenOperand<TokenPlus>(next, level));
                        tokens.Add(exists);
                    }
                    else
                    {
                        // no make sense in practice (circular reference), but is fixes and too get performance
                        if (exists != null && next.HasDirectOrIndirectReference(next))
                        {
                            tokens.Add(AddTokenOperand<TokenPlus>(next, level));
                            tokens.Add(exists);
                        }
                        else
                        {
                            if (next.ProjectsReferences.Count > 0)
                            {
                                tokens.Add(AddTokenOperand<TokenPlus>(next, level));
                                tokens.Add(AddTokenOperand<TokenOpenParenthesis>(next, level));
                                Recursive2(current, next, level);
                                tokens.Add(AddTokenOperand<TokenCloseParenthesis>(next, level));
                            }
                            else
                            {
                                tokens.Add(AddTokenOperand<TokenPlus>(next, level));
                                Recursive2(current, next, level);
                            }
                        }
                    }
                }
            }
        }


        private Cache Recursive(Project left, Events events)
        {
            //if (cacheParsers.ContainsKey(project))
            //    return cacheParsers[project];
            
            cacheParsers[left] = events.InitLeft(left);

            if (left.ProjectsReferences.Count > 0)
            {
                foreach (var right in left.ProjectsReferences)
                {
                    // no make sense in practice (circular reference), but is fixes and too get performance
                    Cache rightResult;
                    cacheParsers.TryGetValue(right, out rightResult);
                    if (rightResult == null)
                        rightResult = this.Recursive(right, events);
                    
                    if (right.ProjectsReferences.Count > 0)
                    {
                        cacheParsers[left].Items.Add(events.InitRightWhenSubExpression(right));
                        cacheParsers[left].Items.Add(rightResult);
                        cacheParsers[left].Items.Add(events.EndRightWhenSubExpression(right));
                    }
                    else 
                    {
                        cacheParsers[left].Items.Add(events.PlusRight(right));
                        cacheParsers[left].Items.Add(rightResult);
                    }
                    
                }
            }

            return cacheParsers[left];
        }

        private object Recursive2(Project project, Func<Project, Project, object> funcToAssignValue)
        {
            //if (cacheParsers.ContainsKey(project))
            //    return cacheParsers[project];

            //cacheParsers[project] = new Cache();
                 
            //if (project.ProjectsReferences.Count > 0)
            //{
            //    foreach (var pRef in project.ProjectsReferences)
            //    {
            //        cacheParsers[project].Items.Add(funcToAssignValue(project, pRef));
            //    }
            //}

            return null;
        }

        private string ToExpression(Project project, Dictionary<Project, string> currentParsedExpression, Action<Project, string> funcToAssignValue)
        {
            var exp = project.Name;
            if (project.ProjectsReferences.Count > 0)
            {
                foreach (var pRef in project.ProjectsReferences)
                {
                    exp += " + ";

                    if (currentParsedExpression.ContainsKey(pRef))
                    {
                        if (showFullTree)
                        {
                            if (pRef.ProjectsReferences.Count == 0)
                                exp += currentParsedExpression[pRef];
                            else
                                exp += "(" + currentParsedExpression[pRef] + ")";
                        }
                        else
                            exp += pRef.Name;
                    }
                    else
                    {
                        if (pRef.ProjectsReferences.Count == 0)
                            exp += ToExpression(pRef, currentParsedExpression, funcToAssignValue);
                        else
                            exp += "(" + ToExpression(pRef, currentParsedExpression, funcToAssignValue) + ")";
                    }
                }
            }

            if (!currentParsedExpression.ContainsKey(project))
                currentParsedExpression.Add(project, exp);

            funcToAssignValue(project, exp);
            
            return exp;
        }
    }
}
