# StringToLinq

StringToLinq is a simple .NET library that transforms string expressions into Linq Expressions. This library enables dynamic generation of complex Linq queries from string inputs, providing a level of flexibility for Linq-based querying.

## Features

- Converts string expressions into Linq Expressions.
- Supports arithmetic, comparison, and logical operators.
- Supports nested types and properties.
- Provides a customizable syntax.
- Fully compatible with Linq's Expression<Func<T, bool>> predicates.

## Quick Start

Install the library through NuGet or by cloning the repository. (Nuget soon)

Define the string that contains the expression you want to parse.

```csharp
string input = "Name eq \"John Doe\" and (Age gt 30 or Department eq \"Sales\")";
```

Use the ToPredicate method from StringExpression to convert the string expression into a Linq Expression.

```csharp
Expression<Func<Employee, bool>> expr = StringExpression.ToPredicate<Employee>(input);
```

Use the generated expression in a Linq query.

```csharp
IEnumerable<Employee> results = employees.AsQueryable().Where(expr);
```

## How it Works

The library works in three steps:

- Parsing the input string into a list of tokens.
- Building an Abstract Syntax Tree (AST) from the tokens.
- Generating a Linq Expression from the AST.

These steps are performed by the ToPredicate method.

## Contributions

Contributions are welcome! Please feel free to submit a pull request.

## License

No license by take the code!
