using Books.Application.Core;

namespace Books.Application;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter file name:");
        string fileName = Console.ReadLine();
        
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string pathToCsvFile = Path.Combine(documentsPath, fileName); 
        string pathToFilterFile = Path.Combine(documentsPath, "filter.json");
        Console.WriteLine($"The file path is: {pathToCsvFile}");
        
        if (!File.Exists(pathToCsvFile) && !File.Exists(pathToFilterFile))
        {
            Console.WriteLine("File not found.");
            return;
        }
        
        string outputPath = Path.Combine(documentsPath, $"output_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

        new FilterBookProcess(pathToCsvFile, pathToFilterFile, outputPath).Process();
    }
}