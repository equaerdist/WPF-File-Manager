using fileChanger.Infrastructure.Commands;
using fileChanger.Services.IUserDialogs;
using fileChanger.Services.IWindowManager;
using fileChanger.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly IWindowManager _windows;

        public DirectoryViewModel CurrentDirectory
        {
            get => _current;
            set
            {
                if (Set(ref _current, value))
                {
                    if (!_history.Contains(value))
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
            {
                _history[index - 1] = RebootDirectoryInfo(_history[index - 1]);
                CurrentDirectory = _history[index - 1];
            }
            if (direction == Direction.Right && index != _history.Count - 1)
            {
                _history[index + 1] = RebootDirectoryInfo(_history[index + 1]);
                CurrentDirectory = _history[index + 1];
            }
        }
        private bool CanExecuteOnSlideDirectory(object? p)
        {
            if (p is null || _history.Count == 0 || _current is null)
                return false;
            var direction = (Direction)p;
            if (direction == Direction.Left && _history.FirstOrDefault() != _current)
                return true;
            if (direction == Direction.Right && _history.Last() != _current)
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
                if (_selectedDirectoryItem != null)
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
        #region ошибки при работе с файлом
        private string FileAccessError(string? additionMessage) => $"Недостаточно прав для записи в файл {additionMessage}";
        private string FileError(string? additionMessage) => $"Непредвиденная ошибка при записи в фаил\n {additionMessage}";
        #endregion
        private async Task HandleFileSafely(Func<Task> func, string sucess, string authException, string others)
        {
            try
            {
                await func();
                _dialogs.ShowInformation(sucess);
            }
            catch (UnauthorizedAccessException ex)
            {
                _dialogs.ShowError($"{authException} {ex.HelpLink}");
            }
            catch (Exception ex)
            {
                _dialogs.ShowError(string.Format("{0} {1}", others, ex.Message));
            }
        }
        #region команда редактирования файла
        public ICommand EditFileCommand { get; set; }

        private async Task WriteContent(string fullName, string? content)
        {
            using var fileStream = File.OpenWrite(fullName);
            using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            await streamWriter.WriteLineAsync(content);
            await fileStream.FlushAsync();
        }
        private async void EditFileCommandExecuted(object? p)
        {
            if (SelectedDirectoryItem is null)
                throw new ArgumentNullException();
            var result = _dialogs.Edit(SelectedDirectoryItem);
            if (result)
            {
                var fileViewModel = (FileViewModel)SelectedDirectoryItem;
                await HandleFileSafely(() => WriteContent(fileViewModel.FullName, fileViewModel.Content),
                    "Фаил успешно отредактирован",
                    "Возникли ошибки досутпа при записи в фаил",
                    "Возникли непредвиденные ошибки при записи в фаил");
            }
        }
        private bool CanEditFileCommandExecute(object? p)
        {
            return SelectedDirectoryItem != null && SelectedDirectoryItem is FileViewModel;
        }
        #endregion
        #region команда удаления файла
        public ICommand DeleteFileCommand { get; set; }
        private bool CanExecuteDeleteFileCommand(object? p) => SelectedDirectoryItem is not null;
        private void DeleteFile(string path) => File.Delete(path);

        private void DeleteDirectory(string path) => Directory.Delete(path, true);
        private DirectoryViewModel RebootDirectoryInfo(DirectoryViewModel info) => new(info.FullName);
        private async void DeleteFileCommandExecuted(object? p)
        {
            if (SelectedDirectoryItem is FileViewModel fileViewModel)
            {
                await HandleFileSafely(() => { DeleteFile(fileViewModel.FullName); return Task.CompletedTask; },
                    "Фаил успешно удален",
                    "Недостаточно прав для удаления файла",
                    "Возникла непредвиденная ошибка при удалении"
                );
                CurrentDirectory = RebootDirectoryInfo(CurrentDirectory);
            }
            if (SelectedDirectoryItem is DirectoryViewModel directoryViewModel)
            {
                await HandleFileSafely(() => { DeleteDirectory(directoryViewModel.FullName); return Task.CompletedTask; },
                    "Директория и все ее содержимое удалено",
                    "Недостаточно прав для удаления директории",
                    "Возникла непредвиденная ошибка при удалении директории"
                );
                CurrentDirectory = RebootDirectoryInfo(CurrentDirectory);
            }
        }
        #endregion
        #region команда добавления файла
        public ICommand CreateFileCommand {get;set;}
        private async Task CreateFileAsync(string path)
        {
            await File.WriteAllTextAsync(path, string.Empty);
        }
        private async void OnCreateFileExecuted(object? p)
        {
            string fileName = string.Empty;
            if (_dialogs.CreateFile(CurrentDirectory.FullName, out fileName))
            {
                await HandleFileSafely(() => CreateFileAsync(fileName),
                    "Фаил успешно создан",
                    "Недостаточно прав для создания файла",
                    "Возникли ошибки при создании файла"
                );
                if(Path.GetDirectoryName(fileName) == CurrentDirectory.FullName)
                    CurrentDirectory = RebootDirectoryInfo(CurrentDirectory);
            }
        }
        private bool CanCreateFileExecute(object? p) => true;
        #endregion
        #region команда открытия файлового менеджера
        public ICommand OpenFileManagerCommand { get; }
        private void OnOpenFileManagerExecute(object? p)
        {
            _windows.OpenWindow(typeof(FileManagerViewModel), null);
        }
        private bool CanExecuteFileManagerOpen(object? p) => true;
        #endregion
        public MainWindowViewModel(IUserDialogs dialogs, IWindowManager windows)
        {
            _dialogs = dialogs;
            _windows = windows;
            CurrentDirectory = new DirectoryViewModel(Environment.GetLogicalDrives().First());
            SlideDirectory = new RelayCommand(OnSlideDirectoryExecuted, CanExecuteOnSlideDirectory);
            EditFileCommand = new RelayCommand(EditFileCommandExecuted, CanEditFileCommandExecute);
            DeleteFileCommand = new RelayCommand(DeleteFileCommandExecuted, CanExecuteDeleteFileCommand);
            CreateFileCommand = new RelayCommand(OnCreateFileExecuted, CanCreateFileExecute);
            OpenFileManagerCommand = new RelayCommand(OnOpenFileManagerExecute, CanExecuteFileManagerOpen);
        }

    }
}
