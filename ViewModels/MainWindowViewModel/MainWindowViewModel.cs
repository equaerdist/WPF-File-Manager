using fileChanger.Infrastructure.Commands;
using fileChanger.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace fileChanger.ViewModels.MainWindowViewModel
{
    public enum Direction { Left, Right }
    class MainWindowViewModel : BaseViewModel
    {
        #region просматриваемая директория
        private DirectoryInfo? _selectedDirectory;

        public DirectoryInfo? SelectedDirectory
        {
            get => _selectedDirectory;
            set
            {
                Set(ref _selectedDirectory, value);
                if (value is not null)
                    CurrentDirectory = value;
            }
        }
        #endregion
        #region История перемещений
        private const int _maxCapacity = 10;
        private List<DirectoryInfo> _history = new();
        #endregion
        private DirectoryInfo _current = null!;
        public DirectoryInfo CurrentDirectory
        {
            get => _current;
            set 
            {
                if (Set(ref _current, value))
                {
                    if(!_history.Contains(value))
                        _history.Add(value);
                    if (_history.Count > _maxCapacity)
                    {
                        var range = _history.Count - _maxCapacity;
                        _history.RemoveRange(0, range);
                    }
                }
            }
        }

        #region Slide Command
        public ICommand SlideDirectory { get; set; }
        private void OnSlideDirectoryExecuted(object? p)
        {
            if (p is null || _current is null)
                throw new ArgumentNullException(nameof(p));
            var direction = (Direction)p;
            var index = _history.FindIndex(d => d.FullName == _current.FullName);
            if (index == -1)
                return;
            if (direction == Direction.Left && index != 0)
                CurrentDirectory = _history[index - 1];
            if (direction == Direction.Right && index != _history.Count - 1)
                CurrentDirectory = _history[index + 1];
        }
        private bool CanExecuteOnSlideDirectory(object? p)
        {
            if (p is null || _history.Count == 0 || _current is null)
                return false;
            var direction = (Direction)p;
            if(direction == Direction.Left && _history.FirstOrDefault() != _current)
                return true;
            if(direction == Direction.Right && _history.Last() != _current)
                return true;
            return false;
        }
        #endregion
        #region древовидная структура файлов
        private object? _selectedDirectoryItem;

        public object? SelectedDirectoryItem
        {
            get => _selectedDirectoryItem ?? _current.EnumerateFiles()
                .Select(f => new FileViewModel(f.FullName)).FirstOrDefault();
            
            set => Set(ref _selectedDirectoryItem, value);
        }
        #endregion

        public MainWindowViewModel()
        {
            CurrentDirectory = new DirectoryInfo(Environment.GetLogicalDrives().First());
            SlideDirectory = new RelayCommand(OnSlideDirectoryExecuted, CanExecuteOnSlideDirectory);
        }

    }
}
