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
        public IEnumerable<DirectoryViewModel> SubDirectories 
        { 
            get
            {
                try
                {
                    return _directory
                    .GetDirectories()
                    .Select(d => new DirectoryViewModel(d.FullName));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Enumerable.Empty<DirectoryViewModel>();
                }
            } 
        }
        public string Name => _directory.Name;
        public string FilesCountInside
        {
            get
            {
                try
                {
                    return _directory.EnumerateFileSystemInfos()
                    .Count()
                    .ToString();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "N/A";
                }
            }
        }
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
            try
            {
                using var file = File.OpenRead(_file.FullName);
                using var reader = new StreamReader(file);
                var buffer = new char[50];
                var readed = await reader.ReadBlockAsync(buffer, 0, 50);
                return new string(buffer) ?? "Некорректный контент";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Не удалось прочитать содержимое";
            }
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
