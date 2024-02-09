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
        }
        Point startPoint;

        void List_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        void List_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed
                && (
                    Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                )
            {
                DataObject dragData = new DataObject(DataFormats.FileDrop, List_left.SelectedItems);
                DragDrop.DoDragDrop(List_left, dragData, DragDropEffects.Copy);
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
    }
}
