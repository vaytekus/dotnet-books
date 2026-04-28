# dotnet-books

Console app that imports books from a CSV file into a SQLite database, filters them by multiple criteria (title, author, genre, publisher, pages, date), and writes the results to an output file.

## Stack

- .NET 9 / C#
- Entity Framework Core + SQLite
- CsvHelper
- BenchmarkDotNet
- xUnit

## Run

```bash
dotnet run --project src/Books.Application
```

## Tests

```bash
dotnet test
```

## Benchmark

```bash
dotnet run --project src/BenchmarkDotNet.Application -c Release
```
