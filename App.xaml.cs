using fileChanger.Services.IUserDialogs;
using fileChanger.ViewModels;
using fileChanger.ViewModels.MainWindowViewMod;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace fileChanger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? _host;
        public static IHost HostClient
        {
            get
            {
                _host ??= CreateHost();
                return _host;
            }
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await HostClient.StartAsync();
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            await HostClient.StopAsync();
            HostClient.Dispose();
        }
        private static IHost CreateHost()
        {
            return Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
                        .ConfigureServices((context, provider) =>
                        {
                            provider.AddSingleton<MainWindowViewModel>();
                            provider.AddSingleton<FileEditorViewModel>();
                            provider.AddSingleton<IUserDialogs, UserDialogs>();
                            context.HostingEnvironment.ContentRootPath = Environment.CurrentDirectory;
                        })
                        .ConfigureAppConfiguration((context, config) =>
                        {
                            config.SetBasePath(Environment.CurrentDirectory);
                            config.AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, "appsettings.json"), false);
                        }).Build();
        }
    }
}
