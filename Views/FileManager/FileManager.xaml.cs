using fileChanger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace fileChanger.Views.FileManager
{
    /// <summary>
    /// Логика взаимодействия для FileManager.xaml
    /// </summary>
    public partial class FileManager : Window
    {
        public FileManager()
        {
          
            InitializeComponent();
            List_left.MouseDown += List_MouseDown;
            List_left.MouseMove += List_MouseMove;
            List_right.MouseDown += List_MouseDown;
            List_right.MouseMove += List_MouseMove;
        }
        Point startPoint;

        void List_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }
        private T? FindChildrenFromType<T>(DependencyObject element) where T : UIElement
        {
            if (element == null)
                return default(T);
            if (element is T)
                return (T)element;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var result = FindChildrenFromType<T>(VisualTreeHelper.GetChild(element, i));
                if (!Equals(default(T), result))
                    return result;
            }
            return default(T);
        }
        private void list_DragOver(object sender, DragEventArgs e)
        {
            var source = e.Source as ListBox;

            if (source == null)
                return;
            var point = e.GetPosition(source);

            var scrollViewer = FindChildrenFromType<ScrollViewer>(source);
            if (scrollViewer == null)
                return;

            if (point.Y < SystemParameters.MinimumVerticalDragDistance)
            {
                scrollViewer.LineUp();
            }
            else if (point.Y > Math.Abs(source.ActualHeight - SystemParameters.MinimumVerticalDragDistance))
            {
                scrollViewer.LineDown();
            }
        }
        void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;
            var target = (ListBox)sender;
            if (e.LeftButton == MouseButtonState.Pressed
                && (
                    Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance
                    && target.SelectedItem is not null)
                )
            {
               
                DataObject dragData = new DataObject(DataFormats.FileDrop, target.SelectedItems);
                DragDrop.DoDragDrop(target, dragData, DragDropEffects.Copy);
            }
        }

        private void List_right_Drop(object sender, DragEventArgs e)
        {
            var item = e.Data.GetData(DataFormats.FileDrop) as IList<object>;
            if (item is null)
                return;
            var dataContext = (FileManagerViewModel)DataContext;
            dataContext.MoveCommand.Execute(new DragDropInfo(ViewModels.MainWindowViewMod.Direction.Left, item));
        }
        private void List_left_Drop(object sender, DragEventArgs e)
        {
            var item = e.Data.GetData(DataFormats.FileDrop) as IList<object>;
            if (item is null)
                return;
            var dataContext = (FileManagerViewModel)DataContext;
            dataContext.MoveCommand.Execute(new DragDropInfo(ViewModels.MainWindowViewMod.Direction.Right, item));
        }
    }
}
