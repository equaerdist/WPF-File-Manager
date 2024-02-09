using fileChanger.Services.IUserDialogs;
using fileChanger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileChanger.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IUserDialogs.IUserDialogs _dialogs;

        public async Task HandleFileSafely(Func<Task> func, string sucess, string authException, string others)
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

        public DirectoryViewModel RebootDirectoryInfo(DirectoryViewModel info) => new(info.FullName);
        public FileService(IUserDialogs.IUserDialogs dialogs)
        {
            _dialogs = dialogs;
        }
    }
}
