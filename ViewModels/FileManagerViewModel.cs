using fileChanger.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		#endregion
		public FileManagerViewModel()
		{
			RightSideDirectory = new DirectoryViewModel(Directory.GetLogicalDrives().First());
			LeftSideDirectory = new DirectoryViewModel(Directory.GetLogicalDrives().First());
		}

	}
}
