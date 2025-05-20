using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Reflection;

namespace Test.Shop.Api.AppStart.FileProvider
{
    /// <summary>
    /// A custom implementation of IFileProvider that provides access to physical files and directories under a specified root path.
    /// It also supports change monitoring using ConfigMapFileProviderChangeToken.
    /// </summary>
    public class ConfigMapFileProvider : IFileProvider
    {
        /// <summary>
        /// A thread-safe dictionary storing file watchers for change tracking.
        /// </summary>
        private readonly ConcurrentDictionary<string, ConfigMapFileProviderChangeToken> watchers;

        /// <summary>
        /// The root directory used as the base for resolving files and directories.
        /// </summary>
        public string RootPath { get; }

        /// <summary>
        /// Constructor that initializes the file provider with the specified root path. Throws an exception if the path is invalid.
        /// </summary>
        /// <param name="rootPath"></param>
        /// <exception cref="ArgumentException"></exception>
        public ConfigMapFileProvider(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                throw new ArgumentException("Invalid root path", nameof(rootPath));
            }

            RootPath = rootPath;
            watchers = new ConcurrentDictionary<string, ConfigMapFileProviderChangeToken>();
        }

        /// <summary>
        /// Creates an instance of ConfigMapFileProvider based on a relative path from the application's executable directory.
        /// </summary>
        /// <param name="subPath"></param>
        /// <returns>Returns null if the path does not exist.</returns>
        public static IFileProvider? FromRelativePath(string subPath)
        {
            var executableLocation = Assembly.GetEntryAssembly()?.Location;
            var executablePath = Path.GetDirectoryName(executableLocation);
            var configPath = Path.Combine(executablePath, subPath);

            if (Directory.Exists(configPath))
            {
                return new ConfigMapFileProvider(configPath);
            }

            return null;
        }

        #region IFileProvider

        /// <summary>
        /// Returns the contents of a directory located at subpath relative to the root path.
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new PhysicalDirectoryContents(Path.Combine(RootPath, subpath));
        }

        /// <summary>
        /// Returns information about a file located at subpath relative to the root path.
        /// </summary>
        /// <param name="subpath"></param>
        /// <returns></returns>
        public IFileInfo GetFileInfo(string subpath)
        {
            return new PhysicalFileInfo(new FileInfo(Path.Combine(RootPath, subpath)));
        }

        /// <summary>
        /// Sets up a file watcher for the specified filter (file path), creating or updating a change token that monitors file modifications.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IChangeToken Watch(string filter)
        {
            var watcher = watchers.AddOrUpdate(filter,
                addValueFactory: (f) =>
                {
                    return new ConfigMapFileProviderChangeToken(RootPath, filter);
                },
                updateValueFactory: (f, e) =>
                {
                    e.Dispose();
                    return new ConfigMapFileProviderChangeToken(RootPath, filter);
                });

            watcher.EnsureStarted();
            return watcher;
        }

        #endregion
    }
}
