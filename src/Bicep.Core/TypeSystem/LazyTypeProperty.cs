// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;

namespace Bicep.Core.TypeSystem
{
    public class LazyTypeProperty : ITypeProperty
    {
        private readonly Lazy<TypeSymbol> lazyType;

        public LazyTypeProperty(string name, Func<TypeSymbol> typeFunc, TypePropertyFlags flags = TypePropertyFlags.None)
        {
            this.Name = name;
            this.lazyType = new Lazy<TypeSymbol>(typeFunc, isThreadSafe: false);
            this.Flags = flags;
        }

        public string Name { get; }

        public TypeSymbol Type => lazyType.Value;

        public TypePropertyFlags Flags { get; }
    }
}
