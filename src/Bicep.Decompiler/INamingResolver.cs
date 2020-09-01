// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Arm.Expression.Expressions;

namespace Bicep.Decompiler
{
    public interface INamingResolver
    {
        string? TryLookupName(NameType nameType, string desiredName);

        string RequestName(NameType nameType, string desiredName);

        string? TryLookupResourceName(string typeString, LanguageExpression nameExpression);

        string RequestResourceName(string typeString, LanguageExpression nameExpression);
    }
}