# Finite Fuzzy Sets for .NET

Fuzzy logic is useful when a value does not fit cleanly into a simple yes/no category. Instead of saying that an element either belongs or does not belong to a set, a fuzzy set assigns a membership grade between `0` and `1`. This makes it possible to model gradual concepts such as low temperature, medium risk, high similarity, or partial truth.

This project provides a small educational .NET library for finite fuzzy sets and the basic operations needed to combine, compare, and inspect them.

# 💡 What Is a Fuzzy Set?

In a classical set, membership is binary: an element is either inside the set or outside it.

In a fuzzy set, membership is gradual:

| Value | Membership in "high temperature" |
| --- | ---: |
| `10 °C` | `0.0` |
| `20 °C` | `0.3` |
| `25 °C` | `0.7` |
| `30 °C` | `1.0` |

The value `25 °C` is therefore not simply "high" or "not high". It belongs to the fuzzy set "high temperature" with grade `0.7`.

This style of modelling is useful for domains where natural language categories are vague, measurements are imprecise, or rules are easier to express in human terms than by hard thresholds.

# 🎯 Overview

FuzzySet started as coursework for fuzzy modelling and is now shaped as a reusable library.

The current version focuses on finite fuzzy sets. It does not contain a demo application yet. The main proof of behavior is the xUnit test suite, and the README includes a compact usage example for working with two fuzzy sets.

The goal of the project is not to become a complete fuzzy logic framework in the first release. The current version provides a clean base for finite fuzzy sets, standard operations, and a future extension toward fuzzy relations.

# When Is This Useful?

This kind of library can be used as a building block for:

* educational experiments with fuzzy set theory
* simple decision models with gradual categories
* comparing vague or partial classifications
* prototyping fuzzy rules before building a larger fuzzy inference system
* future work with fuzzy relations and relation composition

# 🧠 Library Model

The library currently contains these core types:

| Type | Purpose |
| --- | --- |
| `FuzzyElement` | One value and its membership grade in interval `[0, 1]`. |
| `FuzzySet` | Finite fuzzy set storing only elements with positive membership grades. |
| `CrispSet` | Classical set used for universes, cuts, support, kernel, and boundary. |
| `Universe` | Crisp universe over which fuzzy sets can be interpreted. |
| `FuzzyLogic` | Static operations over fuzzy sets. |

Important modelling notes:

* missing values in a `FuzzySet` are interpreted as membership grade `0`
* membership grade `0` is not stored explicitly
* operations over two fuzzy sets validate compatible universes when both sets define one
* public API names are in English, while source comments are Czech because the project keeps its study-project origin

# 🧮 Supported Operations

The public operations are split into two groups: characteristics that describe a single fuzzy set, and operations that create or compare fuzzy sets from existing ones.

## Basic Set Characteristics

| Operation | Method |
| --- | --- |
| kernel / core | `FuzzyLogic.GetKernel(set)` |
| support | `FuzzyLogic.GetSupport(set)` |
| alpha-cut | `FuzzyLogic.GetAlphaCut(set, alpha)` |
| strong alpha-cut | `FuzzyLogic.GetStrongAlphaCut(set, alpha)` |
| boundary | `FuzzyLogic.GetBoundary(set)` |
| height | `FuzzyLogic.GetHeight(set)` |
| normality | `FuzzyLogic.IsNormal(set)` |
| cardinality | `FuzzyLogic.GetCardinality(set)` |
| subset check | `FuzzyLogic.IsSubsetOf(subset, superset)` |

## Fuzzy Set Operations

| Operation | Method | Formula |
| --- | --- | --- |
| standard intersection | `FuzzyLogic.GetStandardIntersection(left, right)` | `min(a, b)` |
| standard union | `FuzzyLogic.GetStandardUnion(left, right)` | `max(a, b)` |
| complement | `FuzzyLogic.GetComplement(universe, set)` | `1 - a` |
| difference | `FuzzyLogic.GetDifference(left, right)` | `min(a, 1 - b)` |
| Lukasiewicz intersection | `FuzzyLogic.GetLukasiewiczIntersection(left, right)` | `max(0, a + b - 1)` |
| Lukasiewicz union | `FuzzyLogic.GetLukasiewiczUnion(left, right)` | `min(1, a + b)` |
| Godel residuum | `FuzzyLogic.GetGodelResiduum(left, right)` | `a <= b ? 1 : b` |
| Lukasiewicz residuum | `FuzzyLogic.GetLukasiewiczResiduum(left, right)` | `min(1, 1 - a + b)` |

The older names `GetRegularIntersection` and `GetRegularUnion` remain available as obsolete compatibility aliases for the standard operations.

# 📐 Mathematical Notes

For binary operations, `a` means the membership grade of a value in the left fuzzy set and `b` means the membership grade of the same value in the right fuzzy set.

Standard union and intersection use the most common fuzzy set definitions:

* union takes the larger membership grade
* intersection takes the smaller membership grade
* complement is calculated relative to a universe
* missing values are treated as membership grade `0`

Residua are implication-like operations. They are included because they are useful in fuzzy logic and will also be relevant if the project later grows toward fuzzy relations.

# ✨ Usage Example

```csharp
using Fuzzy;

Universe universe = new Universe("low", "medium", "high");

FuzzySet temperature = new FuzzySet(universe);
temperature.Add(new FuzzyElement("low", 0.2));
temperature.Add(new FuzzyElement("medium", 0.7));
temperature.Add(new FuzzyElement("high", 1.0));

FuzzySet humidity = new FuzzySet(universe);
humidity.Add(new FuzzyElement("low", 0.8));
humidity.Add(new FuzzyElement("medium", 0.4));
humidity.Add(new FuzzyElement("high", 0.1));

FuzzySet union = FuzzyLogic.GetStandardUnion(temperature, humidity);
FuzzySet intersection = FuzzyLogic.GetStandardIntersection(temperature, humidity);
CrispSet alphaCut = FuzzyLogic.GetAlphaCut(union, 0.7);

double temperatureHeight = FuzzyLogic.GetHeight(temperature);
bool isTemperatureNormal = FuzzyLogic.IsNormal(temperature);
```

# 🗂️ Project Structure

```text
/
|-- CrispSet.cs                  # Classical set implementation
|-- FuzzyElement.cs              # Value with membership grade
|-- FuzzyLogic.cs                # Fuzzy set operations
|-- FuzzySet.cs                  # Finite fuzzy set implementation
|-- Universe.cs                  # Universe of discourse
|
|-- FuzzySet.Tests/              # xUnit test project
|   |-- CrispSetTests.cs
|   |-- FuzzyElementTests.cs
|   |-- FuzzyLogicTests.cs
|   |-- FuzzySetTests.cs
|   `-- SetAssert.cs
|
|-- FuzzySet.csproj
`-- FuzzySet.sln
```

# 🔧 Requirements

* .NET 10 SDK
* Windows, Linux, or macOS supported by the .NET SDK
* Visual Studio 2026 or another editor with .NET support

The library project has no runtime NuGet dependencies.

The test project uses:

* `xunit.v3`
* `Microsoft.NET.Test.Sdk`
* `xunit.runner.visualstudio`

# 🏗️ Build

From the repository root:

```powershell
dotnet restore
dotnet build FuzzySet.sln
```

# 🧪 Testing

Run all tests:

```powershell
dotnet test FuzzySet.sln
```

The test suite currently covers:

* equality and validation for fuzzy elements
* crisp set behavior
* fuzzy set storage rules and membership lookup
* kernel, support, alpha-cuts, boundary, height, normality, and cardinality
* subset checks, complement, difference, unions, intersections, and residua
* invalid input and incompatible universe validation

# ⚠️ Current Limitations

Known limitations:

* the model is currently object-based rather than generic, for example `FuzzySet<string>`
* only finite fuzzy sets are supported
* fuzzy relations are not implemented yet
* there is no demo application or console runner in this release
* advanced t-norms, s-norms, and implication families can be added later

# 🚀 Roadmap

Possible next steps:

* fuzzy relations
* relation composition
* fuzzy equivalence and ordering relations
* additional t-norms, s-norms, and implications
* optional generic API once the core model stabilizes
* small demo project or examples directory if the library grows beyond unit-test examples

# Status

Current status:

* ✅ modernized to `.NET 10`
* ✅ SDK-style project file
* ✅ xUnit v3 test project included
* ✅ public API uses English naming
* ✅ Czech XML comments are included in the source code
* ✅ finite fuzzy set operations are implemented and covered by tests

# License

Educational project intended for study and demonstration purposes.
