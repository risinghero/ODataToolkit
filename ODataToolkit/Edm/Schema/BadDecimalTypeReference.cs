//---------------------------------------------------------------------
// <copyright file="BadDecimalTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ODataToolkit.Validation;

namespace ODataToolkit
{
    /// <summary>
    /// Represents a reference to a semantically invalid EDM decimal type.
    /// </summary>
    internal class BadDecimalTypeReference : EdmDecimalTypeReference, IEdmCheckable
    {
        private readonly IEnumerable<EdmError> errors;

        public BadDecimalTypeReference(string qualifiedName, bool isNullable, IEnumerable<EdmError> errors)
            : base(new BadPrimitiveType(qualifiedName, EdmPrimitiveTypeKind.Decimal, errors), isNullable, null, null)
        {
            this.errors = errors;
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errors; }
        }

        public override string ToString()
        {
            EdmError error = this.Errors.FirstOrDefault();
            Debug.Assert(error != null, "error != null");
            string prefix = error != null ? error.ErrorCode.ToString() + ":" : "";
            return prefix + this.ToTraceString();
        }
    }
}
