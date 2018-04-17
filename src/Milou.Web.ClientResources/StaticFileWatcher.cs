using System;
using System.IO;
using System.Threading;
using JetBrains.Annotations;

namespace Milou.Web.ClientResources
{
    public sealed class StaticFileWatcher : IDisposable
    {
        private readonly Action<string> _onChanged;
        private readonly int _sleepTimeBeforeUpdateInMilliseconds;
        private readonly Action<string> _optionalLogger;
        private FileSystemWatcher _fileSystemWatcher;

        public StaticFileWatcher(Action<string> optionalLogger = null, Action<string> onChanged = null, int sleepTimeBeforeUpdateInMilliseconds = 0)
        {
            _optionalLogger = optionalLogger;
            _onChanged = onChanged;
            this._sleepTimeBeforeUpdateInMilliseconds = sleepTimeBeforeUpdateInMilliseconds;
        }

        public void Watch([NotNull] DirectoryInfo directoryInfo)
        {
            DisposeWatcher();

            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            if (!directoryInfo.Exists)
            {
                throw new ArgumentException($"The directory with path '{directoryInfo.FullName}' does not exist");
            }

            _fileSystemWatcher = new FileSystemWatcher(directoryInfo.FullName)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                                        | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes
                                                        | NotifyFilters.CreationTime | NotifyFilters.Security | NotifyFilters.Size,
                IncludeSubdirectories = true,
                Filter = "*.*"
            };

            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void FileSystemWatcherOnRenamed(object sender, RenamedEventArgs renamedEventArgs)
        {
            Sleep();
            GlobalVersion.Update();
            _optionalLogger?.Invoke($"{renamedEventArgs.OldName} changed to {renamedEventArgs.Name}");
            _onChanged?.Invoke(renamedEventArgs.Name);
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Sleep();
            GlobalVersion.Update();
            _optionalLogger?.Invoke($"{fileSystemEventArgs.Name} was deleted");
            _onChanged?.Invoke(fileSystemEventArgs.Name);
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Sleep();
            GlobalVersion.Update();
            _optionalLogger?.Invoke($"{fileSystemEventArgs.Name} was created");
            _onChanged?.Invoke(fileSystemEventArgs.Name);
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Sleep();
            GlobalVersion.Update();
            _optionalLogger?.Invoke($"{fileSystemEventArgs.Name} changed");
            _onChanged?.Invoke(fileSystemEventArgs.Name);
        }

        private void Sleep()
        {
            if (_sleepTimeBeforeUpdateInMilliseconds > 0)
            {
                Thread.Sleep(_sleepTimeBeforeUpdateInMilliseconds);
            }
        }

        public void Dispose()
        {
            DisposeWatcher();
        }

        private void DisposeWatcher()
        {
            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
                _fileSystemWatcher.Changed -= FileSystemWatcherOnChanged;
                _fileSystemWatcher.Created -= FileSystemWatcherOnCreated;
                _fileSystemWatcher.Deleted -= FileSystemWatcherOnDeleted;
                _fileSystemWatcher.Renamed -= FileSystemWatcherOnRenamed;
                _fileSystemWatcher.Dispose();
            }

            _fileSystemWatcher = null;
        }
    }
}