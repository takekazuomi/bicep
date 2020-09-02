// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.TypeSystem
{
    public class NamedObjectType : ObjectType
    {
        public NamedObjectType(string name, IEnumerable<ITypeProperty> properties, ITypeProperty? additionalProperties)
            : base(name)
        {
            this.Properties = properties.ToImmutableDictionary(property => property.Name, property => property, LanguageConstants.IdentifierComparer);
            this.AdditionalProperties = additionalProperties;
        }

        public override TypeKind TypeKind => TypeKind.NamedObject;

        public override ImmutableDictionary<string, ITypeProperty> Properties { get; }

        public override ITypeProperty? AdditionalProperties { get; }
    }
}
