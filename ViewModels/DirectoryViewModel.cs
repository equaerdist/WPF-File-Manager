using fileChanger.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileChanger.ViewModels
{
    class DirectoryViewModel : BaseViewModel
    {
        private DirectoryInfo _directory;
        public DirectoryViewModel[] SubDirectories => _directory
            .GetDirectories()
            .AsParallel()
            .Select(d => new DirectoryViewModel(d.FullName))
            .ToArray();
        public string Name => _directory.Name;
        public string FilesCountInside => _directory.EnumerateFileSystemInfos()
            .AsParallel()
            .Count()
            .ToString();
        public string LastModified => _directory.LastWriteTime.ToString("hh:mm:ss-dd:MM:yyyy");
        public DirectoryViewModel(string path)
        {
            _directory = new DirectoryInfo(path);
        }
    }
    class FileViewModel : BaseViewModel
    {
        private FileInfo _file;
        public string Name => _file.Name;
        private string _view = string.Empty;
        private async Task<string> GetViewFromFile()
        {
            using var file = File.OpenRead(_file.FullName);
            using var reader = new StreamReader(file);
            var buffer = new char[50];
            var readed = await reader.ReadBlockAsync(buffer, 0, 50);
            return new string(buffer) ?? "Некорректный контент";
        }
        public string LastModified => _file.LastWriteTime.ToString("hh:mm:ss-dd:MM:yyyy");
        public string Size => _file.Length.ToString();
        public string FirstView
        {
            get
            {
                if (_view.Equals(string.Empty))
                    GetViewFromFile().ContinueWith(t => FirstView = t.Result, TaskScheduler.Default);
                return _view;
            }
            set => Set(ref _view, value);
        }
        public FileViewModel(string path)
        {
            _file = new FileInfo(path);
        }
    }
}
