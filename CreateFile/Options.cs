using CommandLine;

namespace CreateFile;

internal class Options
{
    [Option('c', "count", Required = false, HelpText = "Number of files to be generated.")]
    public int Count { get; set; } = 1;

    [Option('s', "size", Required = false, HelpText = "Size of one file.")]
    public double Size { get; set; } = 1;

    [Option('u', "unit", Required = false, HelpText = "Unit of file size.")]
    public SizeUnit SizeUnit { get; set; } = SizeUnit.KiloBytes;

    [Option('n', "name", Required = false, HelpText = "Name[pattern] of file.")]
    public string NamePattern { get; set; } = "testFile.txt";

    [Option('m', "mode", Required = false, HelpText = "File generation mode.")]
    public Mode Mode { get; set; } = Mode.Empty;
}