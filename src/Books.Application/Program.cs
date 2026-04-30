using Books.Application.Core;
using Books.Application.Data;
using Books.Application.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Books.Application;

class Program
{

    static void Main(string[] args)
    {
        var config = ConfigurationHelper.Build();

        string connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");

        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));
        services.AddScoped<FilterBookProcess>();
        var provider = services.BuildServiceProvider();
        
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

        using var scope = provider.CreateScope();
        var process = scope.ServiceProvider.GetRequiredService<FilterBookProcess>();
        process.Process(pathToCsvFile, pathToFilterFile, outputPath);
    }
}