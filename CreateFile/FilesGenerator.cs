using System.Diagnostics;

namespace CreateFile;

internal static class FilesGenerator
{
    public static void GenerateFiles(Options options)
    {
        for (var i = 0; i < options.Count; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var filename = GenerateFilename(options, i);
            var size = options.Size * Math.Pow(1024, (int)options.SizeUnit);
            GenerateFile(filename, (long)size, options.Mode);
            Console.WriteLine($"File '{filename}' generated with size '{options.Size} {options.SizeUnit}' within {stopwatch.Elapsed}.");
        }
    }

    private static string GenerateFilename(Options options, int index)
    {
        if (options.Count > 1)
        {
            var niceIndex = index.ToString($"D{options.Count.ToString().Length}");
            return $"{Path.GetFileNameWithoutExtension(options.NamePattern)}-{niceIndex}{Path.GetExtension(options.NamePattern)}";
        }

        if (options.NamePattern == string.Empty)
        {
            return $"test-{options.Size}-{options.SizeUnit}.{(options.Mode == Mode.Random ? "txt" : "pdf")}";
        }

        return options.NamePattern;
    }

    private static void GenerateFile(string filename, long size, Mode mode)
    {
        switch (mode)
        {
            case Mode.Empty:
                GenerateEmptyFile(filename, size);
                break;
            case Mode.Random:
                GenerateRandomFile(filename, size);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private static void GenerateEmptyFile(string filename, long size)
    {
        using var fs = new FileStream(filename, FileMode.Create);
        fs.Seek(size, SeekOrigin.Begin);
        fs.WriteByte(0);
    }

    private static void GenerateRandomFile(string filename, long size)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();

        using var stream = new StreamWriter(filename);
        for (var i = 0L; i < size; i++)
        {
            stream.Write(chars[random.Next(chars.Length)]);
        }
    }
}