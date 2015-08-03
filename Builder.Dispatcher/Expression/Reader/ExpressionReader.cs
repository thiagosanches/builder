using NCalc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Builder.Dispatcher;



namespace Builder.Core.Expression
{
    /// <summary>
    /// Expression of dependence of objects (EDO) - Reader
    /// </summary>
    public class ExpressionReader
    {
        private ProjectCollection projectCollection;
        private Project project;
        public bool ShowFullTree { get; set; }

        public ExpressionReader(ProjectCollection collection, bool showFullTree = false)
        {
            this.projectCollection = collection;
            this.ShowFullTree = showFullTree;
        }

        public ExpressionReader(Project project, bool showFullTree = false)
        {
            if (project == null)
                throw new Exception("Parameter 'project' can't be null'");

            this.project = project;
            this.ShowFullTree = showFullTree;
        }

        /// <summary>
        /// Return a expression of a project
        /// </summary>
        /// <returns></returns>
        public string ToExpression()
        {
            StringBuilder strBuilder = new StringBuilder();
            var tokens = this.GetTokens();

            foreach(var token in tokens)
            {
                if (token.TokenValue is TokenValuePlus)
                    strBuilder.Append(" + ");
                else if (token.TokenValue is TokenValueOpenParenthesis)
                    strBuilder.Append("(");
                else if (token.TokenValue is TokenValueCloseParenthesis)
                    strBuilder.Append(")");
                else if (token.TokenValue is TokenValueRecursive)
                    strBuilder.Append(((Project)token.TokenValue.Value).Name);
                else
                    strBuilder.Append(((Project)token.TokenValue.Value).Name);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Function to help a debug or undestand a process of the others function
        /// </summary>
        /// <returns></returns>
        public string Debug()
        {
            var tokens = this.GetTokens();

            StringBuilder strBuilder = new StringBuilder();
            foreach (var token in tokens)
            {
                var resParent = "";

                if (token.Parent != null)
                    resParent = string.Format(" parent: (hashcode: {0}; value: {1})", token.Parent.GetHashCode(), token.Parent.TokenValue.ToString());

                strBuilder.Append(token.TokenValue.ToString().Trim());
                strBuilder.Append(string.Format(" hashcode: {0}", token.GetHashCode()));
                strBuilder.Append(resParent);
                strBuilder.Append(" level: " + token.Level);
                strBuilder.Append(" hashcodeValue: " + token.TokenValue.GetHashCode());
                strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }

        #region Parse tokens

        /// <summary>
        /// Get all tokens that represent a expression from the project
        /// </summary>
        /// <param name="project"></param>
        /// <returns>The list of the Token</returns>
        public List<Token> GetTokens()
        {
            var tokens = new List<Token>();
            this.ParseToken(project, null, tokens);
            return tokens;
        }

        /// <summary>
        /// Parse a Token of the project.
        /// </summary>
        /// <param name="project">The project to be converted in a Token</param>
        /// <param name="tokenParent">This parameter is used exclusive in recursive action</param>
        /// <param name="tokenBag">This parameter is used exclusive in recursive action. This is fill in recursive process</param>
        /// <param name="level">The parameter is used exclusive in recursive process</param>
        /// <returns>Return a Token instance that represent a Project instance</returns>
        private Token ParseToken(Project project, Token tokenParent = null, List<Token> tokenBag = null, int level = 1)
        {
            if (tokenBag == null)
                tokenBag = new List<Token>();
            
            Token token = GetOrCreateTokenProject<TokenValue>(project, tokenParent, tokenBag, level);
            tokenBag.Add(token);

            if (project.ProjectsReferences.Count > 0)
            {
                level++;

                foreach (var next in project.ProjectsReferences)
                {
                    // Verify if tokens already exists with the 'next' value
                    // when showFullTree is disabled
                    var exists = tokenBag.FirstOrDefault(f => f.TokenValue.Value == next);
                    if (exists != null && !ShowFullTree)
                    {
                        tokenBag.Add(CreateTokenOperand<TokenValuePlus>(token, level));
                        tokenBag.Add(new Token(exists.TokenValue, token, level));
                    }
                    else
                    {
                        // No make sense in practice (circular reference), 
                        // but is fixes for prevent a infinite call
                        if (exists != null && next.HasDirectOrIndirectReference(next))
                        {
                            tokenBag.Add(CreateTokenOperand<TokenValuePlus>(token, level));
                            tokenBag.Add(new Token(exists.TokenValue, token, level));
                        }
                        else
                        {
                            if (next.ProjectsReferences.Count > 0)
                            {
                                tokenBag.Add(CreateTokenOperand<TokenValuePlus>(token, level));
                                tokenBag.Add(CreateTokenOperand<TokenValueOpenParenthesis>(token, level));
                                this.ParseToken(next, token, tokenBag, level);
                                tokenBag.Add(CreateTokenOperand<TokenValueCloseParenthesis>(token, level));
                            }
                            else
                            {
                                tokenBag.Add(CreateTokenOperand<TokenValuePlus>(token, level));
                                this.ParseToken(next, token, tokenBag, level);
                            }
                        }
                    }
                }
            }

            return token;
        }

        /// <summary>
        /// Create a new Token of types "+", (", ")"
        /// </summary>
        /// <typeparam name="T">The sub type of Token to be a create</typeparam>
        /// <param name="tokenParent">The token parent</param>
        /// <param name="level">The level in expression</param>
        /// <returns>Return a new Token of type T</returns>
        private Token CreateTokenOperand<T>(Token tokenParent, int level) where T : TokenValue
        {
            TokenValue tokenValue = null;
            if (typeof(T) == typeof(TokenValuePlus))
                tokenValue = TokenValuePlus.Instance;
            else if (typeof(T) == typeof(TokenValueOpenParenthesis))
                tokenValue = TokenValueOpenParenthesis.Instance;
            else if (typeof(T) == typeof(TokenValueCloseParenthesis))
                tokenValue = TokenValueCloseParenthesis.Instance;
            else
                throw new Exception(string.Format("Sub type '{0}' of Token is not supported", typeof(T).Name));

            return new Token(tokenValue, tokenParent, level);
        }

        /// <summary>
        /// Create a new Token Project
        /// </summary>
        /// <typeparam name="T">The sub type of Token to be a create</typeparam>
        /// <param name="project">The project to be converted a Token</param>
        /// <param name="projectParent">The token parent</param>
        /// <param name="tokenBag">The token list to help a verify if project already exists and to find a parent token</param>
        /// <param name="level">The level in expression</param>
        /// <returns>Return a new Token of type T</returns>
        private Token GetOrCreateTokenProject<T>(Project project, Token tokenParent, List<Token> tokenBag, int level) where T : TokenValue
        {
            var exists = tokenBag.FirstOrDefault(f => f.TokenValue.Value == project);
            if (exists != null)
                return new Token(exists.TokenValue, tokenParent, level);

            TokenValue tokenValue;

            if (typeof(T) == typeof(TokenValueRecursive))
                tokenValue = new TokenValueRecursive(project);
            else
                tokenValue = new TokenValue(project);

            return new Token(tokenValue, tokenParent, level); ;
        }

        #endregion
    }
}
