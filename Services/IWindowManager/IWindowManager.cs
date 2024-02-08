using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fileChanger.Services.IWindowManager
{
    public interface IWindowManager
    {
        void OpenWindow(Type type, object? owner);
    }
}
