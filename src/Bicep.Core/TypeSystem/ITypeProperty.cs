// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public interface ITypeProperty
    {
        string Name { get; }

        TypeSymbol Type { get; }

        TypePropertyFlags Flags { get; }
    }
}
