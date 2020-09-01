// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Arm.Expression.Expressions;

namespace Bicep.Decompiler
{
    public class UniqueNamingResolver : INamingResolver
    {
        private readonly Dictionary<string, Dictionary<NameType, string>> assignedNames = new Dictionary<string, Dictionary<NameType, string>>(StringComparer.OrdinalIgnoreCase);

        private readonly IExpressionsProvider expressionsProvider;

        public UniqueNamingResolver(IExpressionsProvider expressionsProvider)
        {
            this.expressionsProvider = expressionsProvider;
        }

        private static string EscapeIdentifier(string identifier)
        {
            return Regex.Replace(identifier, "[^a-zA-Z0-9_]", "_");
        }

        public string? TryLookupName(NameType nameType, string desiredName)
        {
            desiredName = EscapeIdentifier(desiredName);

            if (!assignedNames.TryGetValue(desiredName, out var nameByType))
            {
                return null;
            }

            if (!nameByType.TryGetValue(nameType, out var name))
            {
                return null;
            }

            return name;
        }

        public string RequestName(NameType nameType, string desiredName)
        {
            desiredName = EscapeIdentifier(desiredName);

            if (!assignedNames.TryGetValue(desiredName, out var nameByType))
            {
                nameByType = new Dictionary<NameType, string>
                {
                    [nameType] = desiredName,
                };
                assignedNames[desiredName] = nameByType;
            }
            else
            {
                if (nameByType.ContainsKey(nameType))
                {
                    throw new ArgumentException($"Request for duplicate name {desiredName} with type {nameType}");
                }
                
                // TODO technically a naming clash is still possible here but unlikely
                nameByType[nameType] = $"{desiredName}_{nameType.ToString().ToLowerInvariant()}";
            }

            return nameByType[nameType];
        }

        public string? TryLookupResourceName(string typeString, LanguageExpression nameExpression)
        {
            // it's valid to include a trailing slash, so we need to normalize it
            typeString = typeString.TrimEnd('/');

            var nameString = expressionsProvider.SerializeExpression(nameExpression);

            return TryLookupName(NameType.Resource, $"{typeString}_{nameString}");
        }

        public string RequestResourceName(string typeString, LanguageExpression nameExpression)
        {
            // it's valid to include a trailing slash, so we need to normalize it
            typeString = typeString.TrimEnd('/');

            var nameString = expressionsProvider.SerializeExpression(nameExpression);

            return RequestName(NameType.Resource, $"{typeString}_{nameString}");
        }
    }
}