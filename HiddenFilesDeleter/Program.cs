var rootDirectory = @"C:\Users\hsu\OneDrive - Zühlke Engineering AG\Private\2023-01-26";

var files = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

var notDeleted = new List<string>();

foreach (var file in files)
{
    if ((File.GetAttributes(file) & FileAttributes.Hidden) == FileAttributes.Hidden)
    {
        try
        {
            Console.WriteLine(file);
            File.Delete(file);
        }
        catch
        {
            notDeleted.Add(file);
        }
    }
}

Console.WriteLine();
Console.WriteLine("--- DELETION DONE ---");
Console.WriteLine();

foreach (var notDeletedFile in notDeleted)
{
    Console.WriteLine(notDeletedFile);
}

Console.WriteLine();
Console.WriteLine("--- DOME ---");
Console.WriteLine();
