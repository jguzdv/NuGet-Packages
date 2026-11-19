using Karambolo.Extensions.Logging.File;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace JGUZDV.Extensions.Logging
{
    internal class AnyFileProvider : IFileProvider
    {
        public IDirectoryContents GetDirectoryContents(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                throw new ArgumentException($"The path '{path}' is not rooted.", nameof(path));
            }

            return new PhysicalDirectoryContents(path);
        }

        public IFileInfo GetFileInfo(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                throw new ArgumentException($"The path '{path}' is not rooted.", nameof(path));
            }

            return new PhysicalFileInfo(new FileInfo(path));
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }

    internal class PhysicalFileAppender : IFileAppender
    {
        public IFileProvider FileProvider { get; }

        public PhysicalFileAppender()
        {
            FileProvider = new AnyFileProvider();
        }

        public Stream CreateAppendStream(IFileInfo fileInfo)
        {
            return new FileStream(fileInfo.PhysicalPath!, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.SequentialScan | FileOptions.Asynchronous);
        }

        public Task<bool> EnsureDirAsync(IFileInfo fileInfo, CancellationToken cancellationToken = default)
        {
            string dirPath = Path.GetDirectoryName(fileInfo.PhysicalPath)!;
            if (Directory.Exists(dirPath))
                return Task.FromResult(false);

            Directory.CreateDirectory(dirPath);
            return Task.FromResult(true);
        }
    }
}
