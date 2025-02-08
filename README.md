# IsFalse

## 📌 Features

- Provides `IsFalse()` as an intuitive way to check for `false` values.
- Includes a custom analyzer to suggest using `IsFalse()` where appropriate.

## 📦 Installation

Install via NuGet:

```sh
dotnet add package IsFalse
```

## 📖 Usage

```csharp
using IsFalse;

bool condition = false;

if (condition.IsFalse())
{
    Console.WriteLine("The condition is false!");
}
```

## 🔍 Analyzers

This package includes an analyzer (`IsFalse.Analyzers`) that suggests using `someTruth.IsFalse()` instead of `!someTruth`, along with a code fix provider for Intellisense.

## 🕵️ Why?

Sometimes guard clauses or other branching logic can be difficult to read.
They often require us to keep the inverse of the initial condition `if (something)` in mental context when working inside the `else` block:

```csharp
if (something)
{
    DoSomething();
}
else
{
    DoSomethingElse();
}
```

Using two separate conditions and clauses avoids the above problem, but the two conditions are difficult to distinguish visually:

```csharp
if (something)
{
    DoSomething();
}

if (!something)
{
    DoSomethingElse();
}
```

Using the `IsFalse()` method reads more fluently, and allows for granular conditions that are east to keep in mental context.

```csharp
if (something)
{
    DoSomething();
}

if (something.IsFalse())
{
    DoSomethingElse();
}
```

## 💡 Contributions

Feel free to submit issues and pull requests on GitHub!
