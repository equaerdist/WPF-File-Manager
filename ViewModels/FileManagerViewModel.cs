using fileChanger.Infrastructure.Commands;
using fileChanger.Services.IUserDialogs;
using fileChanger.ViewModels.Base;
using fileChanger.ViewModels.MainWindowViewMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace fileChanger.ViewModels
{
   public class FileManagerViewModel : BaseViewModel
    {
        #region выбранная директория правой части
        private DirectoryViewModel _rightSideDirectory = null!;

		public DirectoryViewModel RightSideDirectory
		{
			get { return _rightSideDirectory; }
			set => Set(ref _rightSideDirectory, value);
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
			set => Set(ref _leftSideDirectory, value);
		}

        private readonly IUserDialogs _dialogs;
        #endregion
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
		public FileManagerViewModel(IUserDialogs dialogs)
		{
			_dialogs = dialogs;
			GoBackCommand = new RelayCommand(OnGoBackExecuted, CanExecuteGoBackCommand);
			RightSideDirectory = new DirectoryViewModel(Directory.GetLogicalDrives().First());
			LeftSideDirectory = new DirectoryViewModel(Directory.GetLogicalDrives().First());
		}

	}
}
