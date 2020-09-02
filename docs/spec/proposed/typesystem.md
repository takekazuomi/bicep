# Proposal: Type Population for Resources

In this proposal, I'll discuss the various options we have for exposing type information for resources in the Bicep language. The goal is to provide enhanced information about resource types for IDE completions, validation, and compile-time type checking.

## Packaging
* Types should be exposed via an add-on package for Bicep. This will most likely be a DLL which can be upgraded independently of the CLI.
* This package should contain the ability to:
    * Provide information on the resource types supported by the package.
    * Fetch type information on-demand for a particular resource type.
* Loading of type information should be lazy, to avoid an upfront performance cost of loading types that are not used.
* It should be possible to specify a version of the package to use, so that builds are reproducible. Otherwise if testing in CI and a new package version is picked up, the build may fail unexpectedly.
* It should always be possible to overrule the decisions made by the type system, in case of inaccurate or missing Swagger specs. This can be achieved through use of the `any()` function.

## Type information that is exposed
At a minimum, the type information exposed must allow for:
* Defining `resource` types.
* Read-only, write-only and read-write information about properties.
* The ability to define and reference custom object types as properties.
* Resource functions (e.g. `listKeys()`).
* String literals and discriminated unions.
* Cyclic references between types.

## 3 Options Considered
### Using JSON schema as an intermediary format
Out of these 3 options, I'm currently leaning towards the 3rd (custom serialization/deserialization). For all 3 options, I am assuming that the original source of type information for ARM resources will be Swagger specs.

#### Pros:
* Human readable (somewhat).
* Similar to what we have today with Template Schemas.
* A lot of concepts that we need already exist (readOnly, polymorphism).
* Versioning changes is straightforward.
#### Cons:
* We're introducing an intermediary format between Swagger and the type system. This means we need to handle conversion from Swagger to Schema, and from Schema to Type System.
* JSON Schema is extensible, but any end-to-end functionality will need to be plumbed through as an extension, adding.
* Conversion from Schema to type system must take place at runtime.
* Binary size will likely be larger than other two options.
* With future Bicep extensibility in mind, JSON schema may not adequately represent future 'resource' types added.

### C# Class generation
#### Pros:
* Small binary size, fast to load.
* Type system can be represented at full fidelity without an intermediary format.
#### Cons:
* Versioning will be tricky if we want to version types and CLI independently of each other and make changes to the type system.

### Custom serialization/deserialization mechanism for the type system
#### Pros: 
* Smaller binary size, faster to load than JSON schema.
* Type system can be represented at full fidelity without an intermediary format.
* Intermediary format can be swapped out with relative ease.
#### Cons:
* We need to design this intermediary format and make it resilient to upgrades/downgrades.

## Type system changes required in Bicep
* Support for string literals, so that enum-like capabilities can be achieved with union types (e.g. representing a type that only accepts either "warning" or "error" as `'warning' | 'error'`).
* Support for discriminated unions.