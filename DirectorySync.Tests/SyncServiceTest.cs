namespace DirectorySync.Tests;

[TestClass]
public class SyncServiceTest
{
    [TestMethod]
    public void Sync_FileInSourceOnly_FileIsCopied()
    {
        using var sourceDirectory = TestDirectory.Source().Create().AddFile();
        using var destinationDirectory = TestDirectory.Destination().Create();

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(1, destinationDirectory.GetRelativeFiles().Count());
        Assert.AreEqual(sourceDirectory.GetRelativeFiles().First(), destinationDirectory.GetRelativeFiles().First());
        AssertSyncResult(result, filesAdded: 1);
    }

    [TestMethod]
    public void Sync_DestinationDirectoryDoesNotExist_DestinationIsCreated()
    {
        using var sourceDirectory = TestDirectory.Source().Create();
        using var destinationDirectory = TestDirectory.Destination();

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.IsTrue(Directory.Exists(destinationDirectory.FullName));
        AssertSyncResult(result, directoriesCreated: 1);
    }

    [TestMethod]
    public void Sync_FileInSubDirectory_SubDirectoryIsCreated()
    {
        using var sourceDirectory = TestDirectory.Source().Create().AddDirectory().AddFile();
        using var destinationDirectory = TestDirectory.Destination().Create();

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(1, destinationDirectory.GetRelativeFiles().Count());
        Assert.AreEqual(sourceDirectory.GetRelativeFiles().First(), destinationDirectory.GetRelativeFiles().First());
        AssertSyncResult(result, directoriesCreated: 1, filesAdded: 1);
    }

    [TestMethod]
    public void Sync_FileInDestinationOnly_FileIsDeleted()
    {
        using var sourceDirectory = TestDirectory.Source().Create();
        using var destinationDirectory = TestDirectory.Destination().Create().AddFile();

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(0, destinationDirectory.GetRelativeFiles().Count());
        AssertSyncResult(result, filesDeleted: 1);
    }

    [TestMethod]
    [ExpectedException(typeof(DirectoryNotFoundException))]
    public void Sync_SourceDoesNotExist_ExceptionIsThrown()
    {
        using var sourceDirectory = TestDirectory.Source();
        using var destinationDirectory = TestDirectory.Destination();

        SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);
    }

    [TestMethod]
    public void Sync_SameFileInDestination_FileIsNotCopied()
    {
        using var sourceDirectory = TestDirectory.Source().Create().AddFile();
        using var destinationDirectory = TestDirectory.Destination();
        SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(1, result.FilesNotUpdated.Count);
        AssertSyncResult(result, filesNotUpdated: 1);
    }

    [TestMethod]
    public void Sync_SourceFileLocked_OtherFilesCopied()
    {
        using var sourceDirectory = TestDirectory.Source().Create().AddFile().Lock().AddFile();
        using var destinationDirectory = TestDirectory.Destination().Create();

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(1, destinationDirectory.GetRelativeFiles().Count());
        AssertSyncResult(result, filesAdded: 1, filesNotAdded: 1);
    }

    [TestMethod]
    public void Sync_DestinationFileLocked_OtherFilesCopied()
    {
        using var sourceDirectory = TestDirectory.Source().Create();
        using var destinationDirectory = TestDirectory.Destination().Create().AddFile().Lock();

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(1, destinationDirectory.GetRelativeFiles().Count());
        AssertSyncResult(result, filesNotDeleted: 1);
    }

    [TestMethod]
    public void Sync_HiddenFileInSource_FileIsNotCopied()
    {
        using var sourceDirectory = TestDirectory.Source().Create().AddFile().Hide();
        using var destinationDirectory = TestDirectory.Destination().Create();

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(0, destinationDirectory.GetRelativeFiles().Count());
        AssertSyncResult(result);
    }

    [TestMethod]
    public void Sync_OlderFileInDestination_FileIsReplaced()
    {
        using var sourceDirectory = TestDirectory.Source().Create().AddFile("same");
        using var destinationDirectory = TestDirectory.Destination().Create().AddFile("same").WithContent("other");

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(1, destinationDirectory.GetRelativeFiles().Count());
        AssertSyncResult(result, filesUpdated: 1);
    }

    [TestMethod]
    public void Sync_SameFileButHiddenInSource_FileIsDeleted()
    {
        using var sourceDirectory = TestDirectory.Source().Create().AddFile("same").Hide();
        using var destinationDirectory = TestDirectory.Destination().Create().AddFile("same");

        var result = SyncService.Sync(sourceDirectory.FullName, destinationDirectory.FullName);

        Assert.AreEqual(0, destinationDirectory.GetRelativeFiles().Count());
        AssertSyncResult(result, filesDeleted: 1);
    }

    private static void AssertSyncResult(
        SyncResult result,
        int directoriesCreated = 0,
        int directoriesNotCreated = 0,
        int filesDeleted = 0,
        int filesNotDeleted = 0,
        int filesAdded = 0,
        int filesNotAdded = 0,
        int filesUpdated = 0,
        int filesNotUpdated = 0)
    {
        Assert.AreEqual(directoriesCreated, result.DirectoriesCreated.Count, "Number of 'DirectoriesCreated' invalid.");
        Assert.AreEqual(directoriesNotCreated, result.DirectoriesNotCreated.Count, "Number of 'DirectoriesNotCreated' invalid.");
        Assert.AreEqual(filesDeleted, result.FilesDeleted.Count, "Number of 'FilesDeleted' invalid.");
        Assert.AreEqual(filesNotDeleted, result.FilesNotDeleted.Count, "Number of 'FilesNotDeleted' invalid.");
        Assert.AreEqual(filesAdded, result.FilesAdded.Count, "Number of 'FilesAdded' invalid.");
        Assert.AreEqual(filesNotAdded, result.FilesNotAdded.Count, "Number of 'FilesNotAdded' invalid.");
        Assert.AreEqual(filesUpdated, result.FilesUpdated.Count, "Number of 'FilesUpdated' invalid.");
        Assert.AreEqual(filesNotUpdated, result.FilesNotUpdated.Count, "Number of 'FilesNotUpdated' invalid.");
    }
}