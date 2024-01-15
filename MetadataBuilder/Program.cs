using System.Security.Cryptography;
namespace MetadataBuilder;

internal abstract class Program {

    private static void Main() {
        // Specify the base local directory
        const string localBaseDirectory = "C:/git/GoPokedex/";

        // Specify the base remote URL
        const string remoteBaseUrl = "https://raw.githubusercontent.com/igor-ruivo/go-pokedex/main/";

        // Specify the path for the metadata file
        const string metadataFilePath = "C:/git/metadata.txt";

        // Generate metadata file
        Task.Run(async () => await GenerateMetadataFile(localBaseDirectory, remoteBaseUrl, metadataFilePath)).Wait();

        Console.WriteLine("Metadata file generated successfully.");
    }

    private static async Task GenerateMetadataFile(string localBaseDirectory, string remoteBaseUrl, string metadataFilePath) {
        try {
            await using var writer = new StreamWriter(metadataFilePath);
            // Write the base remote URL to the metadata file
            await writer.WriteLineAsync(remoteBaseUrl);

            // Get all files in the specified local directory and its subdirectories
            var files = Directory.GetFiles(localBaseDirectory, "*.tsx", SearchOption.AllDirectories);

            // Calculate and write hash for each file
            foreach (var localFilePath in files) {
                var relativePath = GetRelativePath(localBaseDirectory, localFilePath);

                var remoteUrl = remoteBaseUrl + relativePath;

                var fileHash = await CalculateMd5FromRemoteUrl(remoteUrl);

                await writer.WriteLineAsync(relativePath);
                await writer.WriteLineAsync(fileHash);
            }
        }
        catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private static string GetRelativePath(string basePath, string fullPath) {
        return Path.GetRelativePath(basePath, fullPath).Replace('\\', '/');
    }

    static async Task<string> CalculateMd5FromRemoteUrl(string remoteUrl) {
        try {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(remoteUrl);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var md5 = MD5.Create();
            var hashBytes = await md5.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        catch (Exception ex) {
            Console.WriteLine($"Error calculating MD5 for {remoteUrl}: {ex.Message}");
            return string.Empty;
        }
    }
}
