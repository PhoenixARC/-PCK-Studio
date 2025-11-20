using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PckStudio.Core.IO
{
    public sealed class FileObserver : IDisposable
    {

        private readonly IDictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();

        public bool AddFileWatcher<T>(string filepath, Action<T, FileInfo> onChange, T value)
            => AddFileWatcher(new FileInfo(filepath), onChange, value);

        public bool AddFileWatcher<T>(FileInfo fileInfo, Action<T, FileInfo> onChange, T value)
        {
            var watcher = new FileSystemWatcher(fileInfo.DirectoryName);

            watcher.NotifyFilter = NotifyFilters.Size | NotifyFilters.CreationTime | NotifyFilters.FileName;

            watcher.Filter = fileInfo.Name;
            watcher.EnableRaisingEvents = true;

            watcher.Error += (e, a) => InternalRemoveFileWatcher(Path.Combine(watcher.Path, watcher.Filter), $"ERROR:({a.GetException().Message})");
            watcher.Deleted += (e, a) => InternalRemoveFileWatcher(Path.Combine(watcher.Path, watcher.Filter), "path deleted");
            watcher.Changed += (e, a) =>
            {
                switch (a.ChangeType)
                {
                    case WatcherChangeTypes.Renamed:
                    case WatcherChangeTypes.Deleted:
                        InternalRemoveFileWatcher(Path.Combine(watcher.Path, watcher.Filter), $"File {a.ChangeType}");
                        return;
                    case WatcherChangeTypes.Changed:
                        onChange(value, new FileInfo(a.FullPath));
                        break;
                }
            };
            if (_watchers.ContainsKey(fileInfo.FullName))
            {
                Debug.WriteLine($"File watcher for: {fileInfo.FullName} already listening.");
                return false;
            }
            _watchers.Add(fileInfo.FullName, watcher);
            Debug.WriteLine($"Added file watcher for: {fileInfo.FullName}.");
            return true;
        }

        public bool RemoveFileWatcher(FileInfo fileInfo) => RemoveFileWatcher(fileInfo.FullName);

        public bool RemoveFileWatcher(string filepath) => InternalRemoveFileWatcher(filepath, "User");

        public void Dispose()
        {
            foreach (KeyValuePair<string, FileSystemWatcher> fsw in _watchers)
            {
                Debug.WriteLine($"Releasing: {fsw.Key}");
                fsw.Value.Dispose();
            }
            _watchers.Clear();
        }

        private bool InternalRemoveFileWatcher(string fullpath, string reason = "None")
        {
            Debug.WriteLine($"Releasing: {fullpath}. Reason: {reason}.");
            _watchers[fullpath]?.Dispose();
            return _watchers.Remove(fullpath);
        }
    }
}
