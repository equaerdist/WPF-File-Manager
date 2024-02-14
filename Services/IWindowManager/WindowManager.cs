using fileChanger.ViewModels;
using fileChanger.Views.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fileChanger.Services.IWindowManager
{
    public  class WindowManager : IWindowManager
    {
        public void OpenWindow(Type type, object? owner)
        {
            if(type == typeof(FileManagerViewModel))
            {
                var owner_window = owner as Window ?? Application.Current.Windows.OfType<Window>().First();
                var manager = new FileManager() { Owner = owner_window };
                manager.Show();
            }
        }
    }
}
