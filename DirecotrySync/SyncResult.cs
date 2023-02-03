namespace DirectorySync;

public class SyncResult
{
    public List<string> DirectoriesCreated { get; } = new();
    public List<string> DirectoriesNotCreated { get; } = new();
    public List<string> FilesDeleted { get; } = new();
    public List<string> FilesNotDeleted { get; } = new();
    public List<string> FilesAdded { get; } = new();
    public List<string> FilesNotAdded { get; } = new();
    public List<string> FilesUpdated { get; } = new();
    public List<string> FilesNotUpdated { get; } = new();

    public void Add(SyncResult otherResult)
    {
        DirectoriesCreated.AddRange(otherResult.DirectoriesCreated);
        DirectoriesNotCreated.AddRange(otherResult.DirectoriesNotCreated);
        FilesDeleted.AddRange(otherResult.FilesDeleted);
        FilesNotDeleted.AddRange(otherResult.FilesNotDeleted);
        FilesAdded.AddRange(otherResult.FilesAdded);
        FilesNotAdded.AddRange(otherResult.FilesNotAdded);
        FilesUpdated.AddRange(otherResult.FilesUpdated);
        FilesNotUpdated.AddRange(otherResult.FilesNotUpdated);
    }
}