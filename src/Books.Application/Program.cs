using Books.Application.Core;
using Books.Application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Books.Application;

class Program
{

    static void Main(string[] args)
    {
        string? env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

        string connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        
        Console.WriteLine("Enter file name:");
        string? fileName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.WriteLine("File name cannot be empty.");
            return;
        }

        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pathToCsvFile = Path.Combine(documentsPath, fileName); 
        string pathToFilterFile = Path.Combine(documentsPath, "filter.json");
        Console.WriteLine($"The file path is: {pathToCsvFile}");
        
        if (!File.Exists(pathToCsvFile))
        {
            Console.WriteLine($"CSV file not found: {pathToCsvFile}");
            return;
        }

        if (!File.Exists(pathToFilterFile))
        {
            Console.WriteLine($"Filter file not found: {pathToFilterFile}");
            return;
        }
        
        string outputPath = Path.Combine(documentsPath, $"output_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

        new FilterBookProcess(pathToCsvFile, pathToFilterFile, outputPath, optionsBuilder.Options).Process();
    }
}