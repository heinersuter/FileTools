namespace DirectorySync;

public class SyncService
{
    public static SyncResult Sync(string sourceDirectory, string destinationDirectory)
    {
        return SyncDirectoryRecursively(sourceDirectory, destinationDirectory);
    }

    private static SyncResult SyncDirectoryRecursively(string sourceDirectory, string destinationDirectory)
    {
        var result = new SyncResult();

        EnsureDirectoryExists(result, destinationDirectory);

        var sourceFiles = Directory.GetFiles(sourceDirectory)
            .Where(file => (File.GetAttributes(file) & FileAttributes.Hidden) != FileAttributes.Hidden)
            .ToList();
        var destinationFiles = Directory.GetFiles(destinationDirectory).AsEnumerable();

        destinationFiles = DeleteFilesInDestination(result, sourceFiles, destinationFiles);

        CopyFiles(result, sourceFiles, destinationDirectory, destinationFiles);

        var subDirectories = Directory.EnumerateDirectories(sourceDirectory);
        foreach (var subDirectory in subDirectories)
        {
            var subDirectoryName = Path.GetFileName(subDirectory);
            var destinationSubDirectory = Path.Combine(destinationDirectory, subDirectoryName);
            result.Add(SyncDirectoryRecursively(subDirectory, destinationSubDirectory));
        }

        return result;
    }

    private static IEnumerable<string> DeleteFilesInDestination(SyncResult result, IEnumerable<string> sourceFiles, IEnumerable<string> destinationFiles)
    {
        var sourceFileNames = sourceFiles.Select(s => Path.GetFileName(s).ToUpperInvariant()).ToHashSet();
        foreach (var destinationFile in destinationFiles)
        {
            var destinationFileName = Path.GetFileName(destinationFile).ToUpperInvariant();
            if (!sourceFileNames.Contains(destinationFileName))
            {
                try
                {
                    File.Delete(destinationFile);
                    result.FilesDeleted.Add(destinationFile);
                }
                catch
                {
                    result.FilesNotDeleted.Add(destinationFile);
                }
            }
            else
            {
                yield return destinationFile;
            }
        }
    }

    private static void CopyFiles(SyncResult result, IEnumerable<string> sourceFiles, string destinationDirectory, IEnumerable<string> destinationFiles)
    {
        var destinationFileNames = destinationFiles.Select(s => Path.GetFileName(s).ToUpperInvariant()).ToHashSet();

        foreach (var sourceFile in sourceFiles)
        {
            var fileName = Path.GetFileName(sourceFile);
            var destinationFile = Path.Combine(destinationDirectory, fileName);

            if (!destinationFileNames.Contains(fileName.ToUpperInvariant()))
            {
                try
                {
                    File.Copy(sourceFile, destinationFile, overwrite: true);
                    result.FilesAdded.Add(destinationFile);
                }
                catch
                {
                    result.FilesNotAdded.Add(destinationFile);
                }
            }
            else
            {
                if (new FileInfo(sourceFile).Length != new FileInfo(destinationFile).Length)
                {
                    File.Copy(sourceFile, destinationFile, overwrite: true);
                    result.FilesUpdated.Add(destinationFile);
                }
                else
                {
                    result.FilesNotUpdated.Add(destinationFile);
                }
            }
        }
    }

    private static void EnsureDirectoryExists(SyncResult result, string destinationDirectory)
    {
        if (!Directory.Exists(destinationDirectory))
        {
            try
            {
                Directory.CreateDirectory(destinationDirectory);
                result.DirectoriesCreated.Add(destinationDirectory);
            }
            catch
            {
                result.DirectoriesNotCreated.Add(destinationDirectory);
            }
        }
    }
}