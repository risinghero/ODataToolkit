//---------------------------------------------------------------------
// <copyright file="CsdlAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace ODataToolkit.Csdl.Parsing.Ast
{
    /// <summary>
    /// Common type for a CSDL annotation.
    /// </summary>
    internal class CsdlAnnotation : CsdlElement
    {
        private readonly CsdlExpressionBase expression;
        private readonly string qualifier;
        private readonly string term;

        public CsdlAnnotation(string term, string qualifier, CsdlExpressionBase expression, CsdlLocation location)
            : base(location)
        {
            this.expression = expression;
            this.qualifier = qualifier;
            this.term = term;
        }

        public CsdlExpressionBase Expression
        {
            get { return this.expression; }
        }

        public string Qualifier
        {
            get { return this.qualifier; }
        }

        public string Term
        {
            get { return this.term; }
        }
    }
}
