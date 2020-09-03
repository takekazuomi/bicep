// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Core.TypeSystem
{
    public class DiscriminatedObjectType : TypeSymbol
    {
        public DiscriminatedObjectType(string name, string discriminatorKey, IEnumerable<ITypeProperty> extendedTypeProperties)
            : base(name)
        {
            // todo validate discriminator types are string literals for now
            this.DiscriminatorKey = discriminatorKey;
            // todo validate that extended types are all of type NamedObjectType
            // but not in the constructor!!
            this.ExtendedTypeProperties = extendedTypeProperties.ToImmutableArray();
        }

        public override TypeKind TypeKind => TypeKind.DiscriminatedObject;

        public string DiscriminatorKey { get; }

        public ImmutableArray<ITypeProperty> ExtendedTypeProperties { get; }

        public IEnumerable<ObjectType> GetExtendedPropertyTypes()
        {
            foreach (var typeProperty in this.ExtendedTypeProperties)
            {
                // only ObjectType children are supported
                // only nested discriminators with the same key are supported
                if (typeProperty.Type is DiscriminatedObjectType child && child.DiscriminatorKey == this.DiscriminatorKey)
                {
                    foreach (var childProperty in child.GetExtendedPropertyTypes())
                    {
                        yield return childProperty;
                    }
                }

                if (typeProperty.Type is ObjectType objectType)
                {
                    yield return objectType;
                }
            }
        }
    }
}
