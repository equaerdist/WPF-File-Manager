using fileChanger.Infrastructure.Commands;
using fileChanger.Services.IUserDialogs;
using fileChanger.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace fileChanger.ViewModels.MainWindowViewMod
{
    public enum Direction { Left, Right }
    public class MainWindowViewModel : BaseViewModel
    {
        #region просматриваемая директория
        private DirectoryViewModel? _selectedDirectory;

        public DirectoryViewModel? SelectedDirectory
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
        private List<DirectoryViewModel> _history = new();
        #endregion
        private DirectoryViewModel _current = null!;
        private readonly IUserDialogs _dialogs;

        public DirectoryViewModel CurrentDirectory
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
            get
            {
                if(_selectedDirectoryItem != null)
                    return _selectedDirectoryItem;
                Task.Run(() =>
                {
                    return _current.DirectoryItems.FirstOrDefault(t => t.GetType() == typeof(FileViewModel));
                }).ContinueWith(t =>
                {
                    if (t.IsFaulted) return;
                    SelectedDirectoryItem = t.Result;
                });
                return null;
            }
            
            set => Set(ref _selectedDirectoryItem, value);
        }
        #endregion
        #region команда редактирования файла
        public ICommand EditFileCommand { get; set; }
        private async Task WriteContent(string fullName, string? content)
        {
            try
            {
                using var fileStream = File.OpenWrite(fullName);
                using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                await streamWriter.WriteLineAsync(content);
                await fileStream.FlushAsync();
                _dialogs.ShowInformation($"Фаил {fullName} успешно записан");
            }
            catch(UnauthorizedAccessException ex)
            {
                _dialogs.ShowError($"Недостаточно прав для записи в файл {ex.Message}");
            }
            catch(Exception ex)
            {
                _dialogs.ShowError($"Непредвиденная ошибка при записи в фаил\n {ex.Message}");
            }
        }
        private async void EditFileCommandExecuted(object? p)
        {
            if (SelectedDirectoryItem is null)
                throw new ArgumentNullException();
            var result = _dialogs.Edit(SelectedDirectoryItem);
            if(result)
            {
                var fileViewModel = (FileViewModel)SelectedDirectoryItem;
                await WriteContent(fileViewModel.FullName, fileViewModel.Content);
            }
        }
        private bool CanEditFileCommandExecute(object? p)
        {
            return SelectedDirectoryItem != null && SelectedDirectoryItem is FileViewModel;
        }
        #endregion
        public MainWindowViewModel(IUserDialogs dialogs)
        {
            _dialogs = dialogs;
            CurrentDirectory = new DirectoryViewModel(Environment.GetLogicalDrives().First());
            SlideDirectory = new RelayCommand(OnSlideDirectoryExecuted, CanExecuteOnSlideDirectory);
            EditFileCommand = new RelayCommand(EditFileCommandExecuted, CanEditFileCommandExecute);
        }

    }
}
