using fileChanger.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace fileChanger.ViewModels
{
    public class FileEditorViewModel : BaseViewModel
    {
		private string _title = string.Empty;
		private FileInfo _file;

		public string Title
		{
			get { return _title; }
			set => Set(ref _title, value);
		}
		private string? _content;
		private async Task InitializeContent()
		{
			try
			{
				using var fileStream = File.Open(_file.FullName, FileMode.Open, FileAccess.Read);
				using var reader = new StreamReader(fileStream, Encoding.UTF8);
				var content = await reader.ReadToEndAsync();
				Content = content;
			}
			catch(UnauthorizedAccessException ex)
			{
				Content = $"Недостаточно прав для просмотра содержимого {ex.HelpLink}";
			}
			catch(Exception ex)
			{
				Content = "Возникла непредвиденная ошибка при чтении";
			}
        }
		public string? Content
		{
			get
			{
				if (_content is not null)
					return _content;
				InitializeContent();
				return _content;
			}
			set => Set(ref _content, value);
		}
		public FileEditorViewModel([CallerFilePath] string? path = null)
		{
			if (path is null) throw new ArgumentNullException();
            _file = new FileInfo(path);
        }

	}
}
