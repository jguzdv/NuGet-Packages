using JGUZDV.OpenIddict.KeyManager.Configuration;
using JGUZDV.OpenIddict.KeyManager.Model;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JGUZDV.OpenIddict.KeyManager.Store
{
    internal abstract class FileStoreBase
    {
        protected readonly IOptions<KeyManagerOptions> _options;
        protected readonly ILogger _logger;

        protected abstract string FileExtension { get; }

        protected FileStoreBase(
            IOptions<KeyManagerOptions> options,
            ILogger logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task<List<KeyInfo>> LoadKeysAsync(KeyUsage keyUsage, CancellationToken ct)
        {
            var keyLoadTasks = EnumerateFiles(keyUsage)
                .Select(x => LoadKeyAsync(x, keyUsage, ct))
                .ToList();

            await Task.WhenAll(keyLoadTasks);

            return keyLoadTasks.Where(x => x.IsCompletedSuccessfully)
                .Select(x => x.Result)
                .ToList();
        }


        private async Task<KeyInfo> LoadKeyAsync(string filepath, KeyUsage keyUsage, CancellationToken ct)
        {
            var bytes = await File.ReadAllBytesAsync(filepath, ct);

            return ConvertFromBytes(bytes, filepath, keyUsage);
        }

        protected abstract KeyInfo ConvertFromBytes(byte[] bytes, string filepath, KeyUsage keyUsage);


        public async Task SaveKeyAsync(KeyInfo keyInfo, CancellationToken ct)
        {
            var filepath = GetKeyFilePath(keyInfo);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filepath)!);
            }
            catch
            {
                _logger.LogInformation("Failed to create directory for key file.");
            }

            var bytes = ConvertToBytes(keyInfo);
            await File.WriteAllBytesAsync(filepath, bytes, ct);
        }

        protected abstract byte[] ConvertToBytes(KeyInfo keyInfo);



        public async Task PurgeExpiredKeys(KeyUsage keyUsage, DateTimeOffset refDate, CancellationToken ct)
        {
            foreach (var fileName in EnumerateFiles(keyUsage).ToList())
            {
                var key = await LoadKeyAsync(fileName, keyUsage, ct);
                if (key.NotAfter > refDate)
                    continue;

                File.Delete(fileName);
            }
        }


        protected virtual string GetStorageDirectory(KeyUsage keyUsage) => Path.Combine(_options.Value.KeyStorePath, GetFolderName(keyUsage));

        protected virtual string GetKeyFilePath(KeyInfo keyInfo) =>
            Path.Combine(GetStorageDirectory(keyInfo.KeyUsage), Path.ChangeExtension(GetKeyFileName(keyInfo), FileExtension));
        
        protected abstract string GetKeyFileName(KeyInfo keyInfo);
        
        protected virtual string GetPattern(KeyUsage keyUsage) => $"*.{FileExtension}";

        private string GetFolderName(KeyUsage keyUsage)
            => keyUsage switch
            {
                KeyUsage.Signature => "sig",
                KeyUsage.Encryption => "enc",
                _ => throw new NotSupportedException($"Key usage '{keyUsage}' is not supported.")
            };

        private IEnumerable<string> EnumerateFiles(KeyUsage keyUsage) => Directory.EnumerateFiles(GetStorageDirectory(keyUsage), GetPattern(keyUsage));
    }
}
