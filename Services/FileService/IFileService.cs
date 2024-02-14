using fileChanger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileChanger.Services.FileService
{
    public interface IFileService
    {
        public DirectoryViewModel RebootDirectoryInfo(DirectoryViewModel directory);
        public Task HandleFileSafely(Func<Task> func, string sucess, string authException, string others);

    }
}
