using fileChanger.ViewModels;
using fileChanger.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fileChanger.Services.IUserDialogs
{
    public class UserDialogs : IUserDialogs
    {
        public bool Confirm(string question)
        {
            return MessageBox.Show(question, "Modal", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK;
        }

        public bool Edit(object item)
        {
            if(item is FileViewModel fileViewModel)
            {
                var viewModelEditor = new FileEditorViewModel(fileViewModel.FullName);
                var dialog = new FileEditor();
                dialog.DataContext = viewModelEditor;
                var result = dialog.ShowDialog();

                if (result != true) return false;
                fileViewModel.Content = viewModelEditor.Content;
                return true;
            }
            throw new ArgumentException(nameof(item));
        }

        public void ShowError(string error)
        {
            MessageBox.Show(error, "Modal", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInformation(string info)
        {
            MessageBox.Show(info, "Modal", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
