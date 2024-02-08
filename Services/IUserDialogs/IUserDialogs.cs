using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileChanger.Services.IUserDialogs
{
    public interface IUserDialogs
    {
        bool Confirm(string question);
        void ShowError(string error);
        bool Edit(object item);
        void ShowInformation(string info);
        bool CreateFile(string initialDirectory, out string fileName);
    }
}
