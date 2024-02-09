using fileChanger.Infrastructure.Commands;
using fileChanger.Services.FileService;
using fileChanger.Services.IUserDialogs;
using fileChanger.ViewModels.Base;
using fileChanger.ViewModels.MainWindowViewMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace fileChanger.ViewModels
{
	public record class DragDropInfo(Direction direction, IList<object> objects);
   public class FileManagerViewModel : BaseViewModel
    {
        #region выбранная директория правой части
        private DirectoryViewModel _rightSideDirectory = null!;

		public DirectoryViewModel RightSideDirectory
		{
			get { return _rightSideDirectory; }
			set
			{
				var result = Set(ref _rightSideDirectory, value);
				if (Sync && result)
                        LeftSideDirectory = value;
			}
		}
		private DirectoryViewModel? _selectedRightSideDirectory;

		public DirectoryViewModel? SelectedRightSideDirectory
		{
			get { return _selectedRightSideDirectory; }
			set
			{
				Set(ref _selectedRightSideDirectory, value);
				if(value  is not null)
					RightSideDirectory = value;
			}
		}
        #endregion
        #region выбранная директория левой чсти
        private DirectoryViewModel? _selectedLeftSideDirectory;

		public DirectoryViewModel? SelectedLeftSideDirectory
		{
			get { return _selectedLeftSideDirectory; }
			set 
			{ 
				Set(ref _selectedLeftSideDirectory, value);
				if(value is not null)
					LeftSideDirectory = value;
			}
		}


		private DirectoryViewModel _leftSideDirectory = null!;

		public DirectoryViewModel LeftSideDirectory
		{
			get { return _leftSideDirectory; }
			set
			{
				if (Set(ref _leftSideDirectory, value) && Sync)
					RightSideDirectory = value;
				
			}
		}
		#endregion
			#region команда получения родителской директории
		public ICommand GoBackCommand { get; }
		private void OnGoBackExecuted(object? p)
		{
			if (p is null) throw new ArgumentNullException();
			Direction direction = (Direction)p;
			DirectoryViewModel workCase = LeftSideDirectory;
			if(direction == Direction.Right)
				workCase = RightSideDirectory;
			var parentRoot = Directory.GetParent(workCase.FullName);
			if (parentRoot is null)
			{
				_dialogs.ShowInformation("Вы уже находитесь в корневом каталоге");
				return;
			}
			if (direction == Direction.Right)
				RightSideDirectory = new DirectoryViewModel(parentRoot.FullName);
			else
				LeftSideDirectory = new DirectoryViewModel(parentRoot.FullName);
		}
		private bool CanExecuteGoBackCommand(object? p) => p is not null && p is Direction;

        private readonly IFileService _fileService;
        private readonly IUserDialogs _dialogs;
        #endregion
        public ICommand MoveCommand { get; }
		private bool CanExecuteMoveCommand(object? p) => true;
		private void OnMoveCommandExecuted(object? p)
		{
			var info = p as DragDropInfo;
			if (info is null) return;
			var list = info.objects;
			for(int i=0; i < list.Count; i++)
			{
				var item = list[i];
                var destination = LeftSideDirectory;
                if (info.direction == Direction.Left)
                    destination = RightSideDirectory;
                if (item is FileViewModel fileViewModel)
				{
					_fileService.HandleFileSafely(
						 () =>
						 {
							 File.Move(fileViewModel.FullName, Path.Combine(destination.FullName, fileViewModel.Name), true);
							 return Task.CompletedTask;
						 },
						"Фаил успешно перемещен",
						"Нет прав в месте назначения",
						"Возникла непредвиденная ошибка при пемещении");
					RebootDirectories();
					
				}
				else if(item is DirectoryViewModel directoryViewModel)
				{
                    _fileService.HandleFileSafely(
                         () =>
                         {
                             Directory.Move(directoryViewModel.FullName, Path.Combine(destination.FullName, directoryViewModel.Name));
                             return Task.CompletedTask;
                         },
                        "Фаил успешно перемещен",
                        "Нет прав в месте назначения",
                        "Возникла непредвиденная ошибка при пемещении");

					RebootDirectories();
				}
			}
			
		}
		private bool _sync;

		public bool Sync
		{
			get { return _sync; }
			set => Set(ref _sync, value);
		}
		private void RebootDirectories()
		{
            RightSideDirectory = _fileService.RebootDirectoryInfo(RightSideDirectory);
            LeftSideDirectory = _fileService.RebootDirectoryInfo(LeftSideDirectory);
        }
        public FileManagerViewModel(IFileService fileService, IUserDialogs dialogs)
		{
			_fileService = fileService;
			_dialogs = dialogs;
			MoveCommand = new RelayCommand(OnMoveCommandExecuted, CanExecuteMoveCommand);
			GoBackCommand = new RelayCommand(OnGoBackExecuted, CanExecuteGoBackCommand);
			RightSideDirectory = new DirectoryViewModel(Directory.GetLogicalDrives().First());
			LeftSideDirectory = new DirectoryViewModel(Directory.GetLogicalDrives().First());
		}

	}
}
