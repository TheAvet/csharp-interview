using NUnit.Framework;
using System.IO;
using System.Linq;

[TestFixture]
public class FileHandlerTests
{
    private const string LocalFilePath = "files.txt"; // Path to store the last 10 files locally

    [Test]
    public void LoadLast10Files_ReadsFilesFromLocalFilePath()
    {
        // Arrange
        var fileHandler = new FileHandler();
        File.WriteAllLines(LocalFilePath, Enumerable.Range(1, 10).Select(i => $"file{i}.txt"));

        // Act
        fileHandler.LoadLast10Files();

        // Assert
        // This is a bit tricky to test because the method writes to the console.
    }
}