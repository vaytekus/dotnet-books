# dotnet-books

Console app that imports books from a CSV file into a SQL Server database, filters them by criteria defined in `filter.json`, and writes results to an output file.

## Stack

- .NET 8 / C#
- Entity Framework Core + SQL Server
- xUnit
- BenchmarkDotNet

## Database

Start SQL Server via Docker:

```bash
docker-compose up -d
```

## Configuration

Create `appsettings.json` in `src/Books.Application/`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=BooksDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

## Run

```bash
dotnet run --project src/Books.Application
```

Enter a CSV file name when prompted — the app looks for it in your Documents folder alongside `filter.json`.

## Test

```bash
dotnet test
```