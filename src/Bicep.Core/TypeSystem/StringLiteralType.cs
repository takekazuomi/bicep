// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class StringLiteralType : TypeSymbol
    {
        public StringLiteralType(string value)
            : base(value)
        {
        }

        public override TypeKind TypeKind => TypeKind.Literal;
    }
}