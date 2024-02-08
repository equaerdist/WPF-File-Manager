using fileChanger.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileChanger.ViewModels
{
    internal class FileManagerViewModel : BaseViewModel
    {
		private DirectoryViewModel _rightSideDirectory = null!;

		public DirectoryViewModel RightSideDirectory
		{
			get { return _rightSideDirectory; }
			set => Set(ref _rightSideDirectory, value);
		}
		private DirectoryViewModel _leftSideDirectory = null!;

		public DirectoryViewModel LeftSideDirectory
		{
			get { return _leftSideDirectory; }
			set => Set(ref _leftSideDirectory, value);
		}
		public FileManagerViewModel()
		{
			RightSideDirectory = new DirectoryViewModel(Directory.GetLogicalDrives().First());
			LeftSideDirectory = new DirectoryViewModel(Directory.GetLogicalDrives().First());
		}

	}
}
