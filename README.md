# Finite Fuzzy Sets and Relations for .NET

Fuzzy logic is useful when a value does not fit cleanly into a simple yes/no category. Instead of saying that an element either belongs or does not belong to a set, a fuzzy set assigns a membership grade between `0` and `1`. This makes it possible to model gradual concepts such as low temperature, medium risk, high similarity, or partial truth.

This project provides a small educational .NET library for finite fuzzy sets, binary fuzzy relations, defuzzification, and the basic operations needed to combine, compare, inspect, and compose them.

# 💡 What Is a Fuzzy Set?

In a classical set, membership is binary: an element is either inside the set or outside it.

In a fuzzy set, membership is gradual. For example, the fuzzy set "high temperature" could assign these grades:

| Value | Membership grade |
| --- | ---: |
| `10 °C` | `0.0` |
| `20 °C` | `0.3` |
| `25 °C` | `0.7` |
| `30 °C` | `1.0` |

The value `25 °C` is therefore not simply "high" or "not high". It belongs to the fuzzy set "high temperature" with grade `0.7`.

This style of modelling is useful for domains where natural language categories are vague, measurements are imprecise, or rules are easier to express in human terms than by hard thresholds.

# 🎯 Overview

FuzzySet started as coursework for fuzzy modelling and is now shaped as a reusable library.

The current version focuses on finite fuzzy sets, binary fuzzy relations, and discrete defuzzification methods. It does not contain a demo application yet. The main proof of behavior is the xUnit test suite, and the README includes compact usage examples for working with fuzzy sets and relations.

The goal of the project is not to become a complete fuzzy logic framework in the first release. The current version provides a clean base for finite fuzzy sets, standard operations, and the first layer of relation operations.

# When Is This Useful?

This kind of library can be used as a building block for:

* educational experiments with fuzzy set theory
* simple decision models with gradual categories
* comparing vague or partial classifications
* modelling gradual relationships between two universes
* prototyping fuzzy rules before building a larger fuzzy inference system
* experimenting with max-min relation composition
* converting fuzzy outputs back to crisp numeric values

# 🧠 Library Model

The library currently contains these core types:

| Type | Purpose |
| --- | --- |
| `FuzzyElement` | One value and its membership grade in interval `[0, 1]`. |
| `FuzzySet` | Finite fuzzy set storing only elements with positive membership grades. |
| `CrispSet` | Classical set used for universes, cuts, support, kernel, and boundary. |
| `Universe` | Crisp universe over which fuzzy sets can be interpreted. |
| `FuzzyLogic` | Static operations over fuzzy sets. |
| `FuzzyRelationElement` | One pair of values and its membership grade in a binary fuzzy relation. |
| `FuzzyRelation` | Finite binary fuzzy relation over two universes. |
| `FuzzyRelationLogic` | Static operations over binary fuzzy relations. |
| `FuzzyDefuzzification` | Static defuzzification methods for finite fuzzy sets. |

Important modelling notes:

* missing values in a `FuzzySet` are interpreted as membership grade `0`
* membership grade `0` is not stored explicitly
* missing pairs in a `FuzzyRelation` are interpreted as membership grade `0`
* operations over two fuzzy sets validate compatible universes when both sets define one
* relation composition validates compatible middle universes when both relations define one
* defuzzification requires a value selector that maps set values to a numeric axis
* public API names are in English, while source comments are Czech because the project keeps its study-project origin

# 🧮 Supported Operations

The public operations are split into four groups: characteristics that describe a single fuzzy set, operations that create or compare fuzzy sets, operations over binary fuzzy relations, and defuzzification methods.

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

## Fuzzy Relation Operations

| Operation | Method | Formula |
| --- | --- | --- |
| Cartesian product | `FuzzyRelationLogic.GetCartesianProduct(left, right)` | `min(a, b)` |
| left projection | `FuzzyRelationLogic.GetLeftProjection(relation)` | `max_y R(x, y)` |
| right projection | `FuzzyRelationLogic.GetRightProjection(relation)` | `max_x R(x, y)` |
| image | `FuzzyRelationLogic.GetImage(set, relation)` | `max_x min(A(x), R(x, y))` |
| composition | `FuzzyRelationLogic.GetComposition(left, right)` | `max_y min(R(x, y), S(y, z))` |

## Defuzzification Methods

| Method | Operation |
| --- | --- |
| COG | `FuzzyDefuzzification.GetCenterOfGravity(set, selector)` |
| COA | `FuzzyDefuzzification.GetCenterOfArea(set, selector)` |
| COS | `FuzzyDefuzzification.GetCenterOfSums(sets, selector)` |
| MOM | `FuzzyDefuzzification.GetMeanOfMaxima(set, selector)` |
| FOM | `FuzzyDefuzzification.GetFirstOfMaxima(set, selector)` |
| LOM | `FuzzyDefuzzification.GetLastOfMaxima(set, selector)` |

# 📐 Mathematical Notes

For binary operations, `a` means the membership grade of a value in the left fuzzy set and `b` means the membership grade of the same value in the right fuzzy set.

Standard union and intersection use the most common fuzzy set definitions:

* union takes the larger membership grade
* intersection takes the smaller membership grade
* complement is calculated relative to a universe
* missing values are treated as membership grade `0`

Residua are implication-like operations. They are included because they are useful in fuzzy logic and can also be used in future extensions around fuzzy rules.

For binary fuzzy relations, `R(x, y)` means the membership grade of the pair `(x, y)` in relation `R`. Projections summarize a relation back into a fuzzy set, and composition links two relations through a shared middle universe.

Defuzzification converts a fuzzy set back to one crisp numeric value. Because `FuzzySet` can store any object values, each defuzzification method receives a selector such as `x => Convert.ToDouble(x)`.

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

Relation example:

```csharp
Universe inputs = new Universe("low", "medium", "high");
Universe outputs = new Universe("slow", "normal", "fast");

FuzzySet temperature = new FuzzySet(inputs);
temperature.Add(new FuzzyElement("low", 0.2));
temperature.Add(new FuzzyElement("medium", 0.7));
temperature.Add(new FuzzyElement("high", 1.0));

FuzzyRelation speedRelation = new FuzzyRelation(inputs, outputs);
speedRelation.Add(new FuzzyRelationElement("low", "slow", 0.8));
speedRelation.Add(new FuzzyRelationElement("medium", "normal", 0.9));
speedRelation.Add(new FuzzyRelationElement("high", "fast", 1.0));

FuzzySet recommendedSpeed = FuzzyRelationLogic.GetImage(temperature, speedRelation);
FuzzySet outputCoverage = FuzzyRelationLogic.GetRightProjection(speedRelation);
```

Defuzzification example:

```csharp
Universe fanSpeed = new Universe(0, 25, 50, 75, 100);

FuzzySet speedOutput = new FuzzySet(fanSpeed);
speedOutput.Add(new FuzzyElement(25, 0.4));
speedOutput.Add(new FuzzyElement(50, 0.8));
speedOutput.Add(new FuzzyElement(75, 0.8));

double centerOfGravity = FuzzyDefuzzification.GetCenterOfGravity(speedOutput, x => Convert.ToDouble(x));
double meanOfMaxima = FuzzyDefuzzification.GetMeanOfMaxima(speedOutput, x => Convert.ToDouble(x));
```

# 🗂️ Project Structure

```text
/
├── CrispSet.cs                  # Classical set implementation
├── FuzzyElement.cs              # Value with membership grade
├── FuzzyDefuzzification.cs      # Defuzzification methods
├── FuzzyLogic.cs                # Fuzzy set operations
├── FuzzyRelation.cs             # Binary fuzzy relation implementation
├── FuzzyRelationElement.cs      # Pair of values with membership grade
├── FuzzyRelationLogic.cs        # Fuzzy relation operations
├── FuzzySet.cs                  # Finite fuzzy set implementation
├── Universe.cs                  # Universe of discourse
│
├── FuzzySet.Tests/              # xUnit test project
│   ├── CrispSetTests.cs
│   ├── FuzzyDefuzzificationTests.cs
│   ├── FuzzyElementTests.cs
│   ├── FuzzyLogicTests.cs
│   ├── FuzzyRelationElementTests.cs
│   ├── FuzzyRelationLogicTests.cs
│   ├── FuzzyRelationTests.cs
│   ├── FuzzySetTests.cs
│   └── SetAssert.cs
│
├── FuzzySet.csproj
└── FuzzySet.sln
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
* binary fuzzy relation storage and membership lookup
* Cartesian product, projections, image, and max-min relation composition
* COG/COA, COS, MOM, FOM, and LOM defuzzification
* invalid input and incompatible universe validation

# ⚠️ Current Limitations

Known limitations:

* the model is currently object-based rather than generic, for example `FuzzySet<string>` or `FuzzyRelation<string, int>`
* only finite fuzzy sets are supported
* only binary fuzzy relations are supported
* defuzzification is discrete and requires a numeric value selector
* there is no demo application or console runner in this release
* advanced t-norms, s-norms, and implication families can be added later

# 🚀 Roadmap

Possible next steps:

* n-ary fuzzy relations
* relation properties such as reflexivity, symmetry, and transitivity
* fuzzy equivalence and ordering relations
* additional relation composition variants
* additional t-norms, s-norms, and implications
* optional generic API once the core model stabilizes
* small demo project or examples directory if the library grows beyond unit-test examples

# Status

Current status:

* ✅ public API uses English naming
* ✅ Czech XML comments are included in the source code
* ✅ finite fuzzy sets are implemented with basic characteristics and set operations
* ✅ binary fuzzy relations are implemented with projections, image, and max-min composition
* ✅ discrete defuzzification methods are implemented
* ✅ behavior is covered by an xUnit test suite

# License

Educational project intended for study and demonstration purposes.
