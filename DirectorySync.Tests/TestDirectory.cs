namespace DirectorySync.Tests;

public class TestDirectory : IDisposable
{
    private readonly DirectoryInfo _directory;
    private readonly List<FileStream> _lockedFiles = new();

    private string _currentDirectory;
    private string? _currentFile;

    private TestDirectory(string name)
    {
        _directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "_SyncServiceTest_", $"{name}-{GetRandomName()}"));
        _currentDirectory = _directory.FullName;
    }

    public string FullName => _directory.FullName;

    public static TestDirectory Source()
    {
        return new TestDirectory("Source");
    }

    public static TestDirectory Destination()
    {
        return new TestDirectory("Destination");
    }

    public TestDirectory Create()
    {
        _directory.Create();
        return this;
    }

    public TestDirectory AddFile(string? fileName = null)
    {
        fileName ??= $"File-{GetRandomName()}.txt";
        var file = Path.Combine(_currentDirectory, fileName);
        File.WriteAllText(file, "Test File Content");
        _currentFile = file;
        return this;
    }

    public TestDirectory AddDirectory()
    {
        var newDirectory = Path.Combine(_currentDirectory, $"Directory-{GetRandomName()}");
        Directory.CreateDirectory(newDirectory);
        _currentDirectory = newDirectory;
        return this;
    }

    public TestDirectory Lock()
    {
        if (_currentFile != null && File.Exists(_currentFile))
        {
            var fileStream = new FileStream(_currentFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            _lockedFiles.Add(fileStream);
        }

        return this;
    }

    public TestDirectory Hide()
    {
        if (_currentFile != null && File.Exists(_currentFile))
        {
            File.SetAttributes(_currentFile, FileAttributes.Hidden);
        }

        return this;
    }

    public TestDirectory WithContent(string content)
    {
        if (_currentFile != null && File.Exists(_currentFile))
        {
            File.WriteAllText(_currentFile, content);
        }

        return this;
    }

    public IEnumerable<string> GetRelativeFiles()
    {
        return _directory.GetFiles("*.*", SearchOption.AllDirectories)
            .Select(info => info.FullName[_directory.FullName.Length..])
            .OrderByDescending(s => s.Split('\\').Length)
            .ThenBy(s => s);
    }

    private string GetRandomName()
    {
        return Guid.NewGuid().ToString()[^12..];
    }

    private void ReleaseUnmanagedResources()
    {
        if (Directory.Exists(_directory.FullName))
        {
            _lockedFiles.ForEach(fileStream => fileStream.Close());
            _directory.Delete(recursive: true);
        }
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~TestDirectory()
    {
        ReleaseUnmanagedResources();
    }
}