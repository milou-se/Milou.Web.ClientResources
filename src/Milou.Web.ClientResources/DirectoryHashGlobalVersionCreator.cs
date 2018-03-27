using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace Milou.Web.ClientResources
{
    public class DirectoryHashGlobalVersionCreator : IGlobalVersionCreator
    {
        private readonly Action<string> _debugLog;

        private static readonly List<string> _excludedFileTypes = new List<string> { ".lock", ".tmp", ".ide", ".user", ".ide-shm", ".ide-wal" };

        public DirectoryHashGlobalVersionCreator([NotNull] string absoluteBaseDirectoryPath)
            : this(absoluteBaseDirectoryPath, _ => { })
        {
            if (string.IsNullOrWhiteSpace(absoluteBaseDirectoryPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(absoluteBaseDirectoryPath));
            }
        }

        public DirectoryHashGlobalVersionCreator([NotNull] string absoluteBaseDirectoryPath, Action<string> debugLog)
        {
            if (string.IsNullOrWhiteSpace(absoluteBaseDirectoryPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(absoluteBaseDirectoryPath));
            }

            if (debugLog == null)
            {
                throw new ArgumentNullException(nameof(debugLog));
            }

            _debugLog = debugLog;

            if (!Directory.Exists(absoluteBaseDirectoryPath))
            {
                throw new InvalidOperationException(
                    $"The specified physical path '{absoluteBaseDirectoryPath}' does not exist");
            }

            AbsoluteBaseDirectoryPath = absoluteBaseDirectoryPath;
        }

        public string AbsoluteBaseDirectoryPath { get; }

        public string CreateGlobalId()
        {
            string hash = CreateHash(AbsoluteBaseDirectoryPath);

            return hash;
        }

        private string CreateHash(string directoryPath)
        {
            if (!Directory.Exists(AbsoluteBaseDirectoryPath))
            {
                throw new InvalidOperationException(
                    $"The specified physical path '{AbsoluteBaseDirectoryPath}' does not exist");
            }

            _debugLog($"Creating hash based on files in directory '{directoryPath}'");

            Stopwatch totalStopwatch = Stopwatch.StartNew();

            List<string> files =
                Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories).OrderBy(file => file).ToList();

            string hash = "";

            if (files.Any())
            {
                _debugLog($"{files.Count} files exist in total for directory '{directoryPath}' and its subdirectories");

                using (MD5 hashAlgorithm = MD5.Create())
                {
                    int processedFileCounter = 0;

                    foreach (var fileIterator in files.Select((file, index) => new { FilePath = file, Index = index }))
                    {
                        if (File.Exists(fileIterator.FilePath))
                        {
                            string currentExtension = Path.GetExtension(fileIterator.FilePath);
                            if (_excludedFileTypes.Any(extension => extension.Equals(currentExtension, StringComparison.OrdinalIgnoreCase)))
                            {
                                _debugLog($"Skipping file {fileIterator.FilePath}, black listed extension '{currentExtension}'");
                                continue;
                            }

                            Stopwatch stopwatch = Stopwatch.StartNew();

                            string relativePath = fileIterator.FilePath.Substring(directoryPath.Length + 1);
                            byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLowerInvariant());
                            hashAlgorithm.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                            byte[] contentBytes = File.ReadAllBytes(fileIterator.FilePath);

                            if (fileIterator.Index == files.Count - 1)
                            {
                                hashAlgorithm.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                            }
                            else
                            {
                                hashAlgorithm.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                            }

                            stopwatch.Stop();

                            _debugLog(
                                $"Calculated hash for {fileIterator.FilePath} in {stopwatch.Elapsed.TotalMilliseconds:F} ms");

                            processedFileCounter++;
                        }
                    }
                    if (processedFileCounter > 0)
                    {
                        hash = BitConverter.ToString(hashAlgorithm.Hash).Replace("-", "");
                    }
                }

                totalStopwatch.Stop();
            }
            else
            {
                _debugLog($"No files exist in directory '{directoryPath}' or in any of its subdirectories");

                using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
                {
                    var randomBytes = new byte[16];
                    cryptoServiceProvider.GetBytes(randomBytes);

                    hash = BitConverter.ToString(randomBytes).Replace("-", "");
                }
            }

            _debugLog($"Total time for creating hash: {totalStopwatch.Elapsed.TotalSeconds:F} s");

            return hash;
        }
    }
}