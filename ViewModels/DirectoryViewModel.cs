using fileChanger.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace fileChanger.ViewModels
{
    public class DirectoryViewModel : BaseViewModel
    {
        private DirectoryInfo _directory;
        public string FullName => _directory.FullName;
        private IEnumerable<object>? _directoryItems;
        private async void InitializeItems()
        {
            try
            {
                var items = await Task.Run(() => Enumerable.Cast<object>(_directory.EnumerateFiles().Select(t => new FileViewModel(t.FullName)))
                        .Concat(_directory.EnumerateDirectories().Select(t => new DirectoryViewModel(t.FullName))));
                DirectoryItems = items;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public IEnumerable<object> DirectoryItems { 
            get 
            {
               
                if (_directoryItems == null)
                {
                    InitializeItems();
                    return Enumerable.Empty<object>();
                }
                return _directoryItems;
            }
            set => Set(ref _directoryItems, value);
        }
        private IEnumerable<DirectoryViewModel>? _subDirectories;
        public IEnumerable<DirectoryViewModel> SubDirectories 
        { 
            get
            {
                if (_subDirectories is not null) return _subDirectories;
               
                    Task.Run(() => _directory
                    .GetDirectories()
                    .Select(d => new DirectoryViewModel(d.FullName))).ContinueWith(t =>
                    {
                        if (t.IsFaulted) return;
                        SubDirectories = t.Result;
                    });
                    return Enumerable.Empty<DirectoryViewModel>();
            }
            set => Set(ref _subDirectories, value);
        }
        public string Name => _directory.Name;
        public string? _filesCountInside;
        public override bool Equals(object? obj)
        {
            if(obj is DirectoryViewModel directoryViewModel)
            {
                return directoryViewModel.FullName == FullName;
            }
            return base.Equals(obj);
        }
        public string FilesCountInside
        {
            get
            {
                if (_filesCountInside is not null) return _filesCountInside;
                Task.Run(() => _directory.EnumerateFileSystemInfos()
                .Count()
                .ToString()).ContinueWith(t =>
                {
                    if (t.IsFaulted) return;
                    FilesCountInside = t.Result;
                });
                return "N/A";
            }
            set => Set(ref _filesCountInside, value);
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
        public string FullName => _file.FullName;
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
        public string Size { get 
            {

                try
                {
                    return _file.Length.ToString();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "Не удалось вычислить размер файла";
                }
            } 
        }
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
        public string? Content { get; set; }
        public FileViewModel(string path)
        {
            _file = new FileInfo(path);
        }
    }
}
