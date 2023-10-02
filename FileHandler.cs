using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class FileHandler
{
    private const string LocalFilePath = "files.txt"; // Path to store the last 10 files locally
    private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(3); // To limit parallel downloads
    private static readonly HttpClient Client = new HttpClient();

    public void Start()
    {
        LoadLast10Files(); // Load the last 10 files on app startup

        // Refresh the list of files every 1 minute(5 is too long for testing purposes)
        var refreshTimer = new Timer(callback: RefreshFiles, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }

    public void LoadLast10Files()
    {
        if (File.Exists(LocalFilePath))
        {
            var files = File.ReadLines(LocalFilePath).ToList();
            Console.WriteLine("Last 10 files locally stored:");
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }
        }
    }

    public async Task DownloadFile(Uri file)
    {
        await Semaphore.WaitAsync(); // Acquire a permit for parallel download
        try
        {
            var localPath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(file.LocalPath));
            if (File.Exists(localPath))
            {
                Console.WriteLine($"File already exists, skipping download: {file}");
                return;
            }

            // Download file
            Console.WriteLine($"Downloading file: {file}");
            var bytes = await Client.GetByteArrayAsync(file);
            await File.WriteAllBytesAsync(localPath, bytes);

            Console.WriteLine($"Downloaded file: {file}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while downloading file: {file} - {ex.Message}");
        }
        finally
        {
            Semaphore.Release(); // Release the permit
        }
    }

    private void RefreshFiles(object state)
    {
        try
        {
            var provider = new AisUriProvider();
            var files = provider.Get().Take(10).ToList(); // Get the latest 10 files

            Console.WriteLine("Latest 10 files:");
            foreach (var file in files)
            {
                Console.WriteLine(file);
            }

            // Download the files in parallel with a limit of 3 simultaneous downloads
            var downloadTasks = files.Select(file => DownloadFile(file));
            Task.WaitAll(downloadTasks.ToArray());

            // Save the downloaded files locally
            File.WriteAllLines(LocalFilePath, files.Select(file => Path.GetFileName(file.LocalPath)));

            // Delete unnecessary/old files
            var oldFiles = File.ReadLines(LocalFilePath)
                .Except(files.Select(file => Path.GetFileName(file.LocalPath)))
                .ToList();

            foreach (var oldFile in oldFiles)
            {
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), oldFile));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while refreshing files: {ex.Message}");
        }
    }
}