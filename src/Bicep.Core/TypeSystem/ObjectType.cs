// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    /// <summary>
    /// Represents an object with any property of any type.
    /// </summary>
    public class ObjectType : TypeSymbol
    {
        private static readonly ITypeProperty DefaultAdditionalProperties = new TypeProperty(
            "additionalProperties",
            LanguageConstants.Any,
            TypePropertyFlags.None);

        public ObjectType(string name) : base(name)
        {
        }

        public override TypeKind TypeKind => TypeKind.Primitive;

        public virtual ImmutableDictionary<string, ITypeProperty> Properties => ImmutableDictionary<string, ITypeProperty>.Empty;

        public virtual ITypeProperty? AdditionalProperties => DefaultAdditionalProperties;
    }
}
