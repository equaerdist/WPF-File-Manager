using fileChanger.ViewModels.MainWindowViewMod;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fileChanger.ViewModels
{
    public class ViewModelAllocator
    {
        public MainWindowViewMod.MainWindowViewModel MainWindowViewModel => App.HostClient.Services
            .GetRequiredService<MainWindowViewModel>();
        public FileManagerViewModel FileManagerViewModel => App.HostClient.Services .GetRequiredService<FileManagerViewModel>();
    }
}
