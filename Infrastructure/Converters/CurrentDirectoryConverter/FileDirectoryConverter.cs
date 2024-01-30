using fileChanger.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace fileChanger.Infrastructure.Converters.CurrentDirectoryConverter
{
    class FileDirectoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var directoryInfo = value as DirectoryInfo;
            if (directoryInfo is null)
                throw new ArgumentNullException($"{nameof(directoryInfo)}_____{value.GetType().Name}");
            try 
            { 
                return Enumerable.Cast<object>(directoryInfo.EnumerateFiles()
                    .Select(t => new FileViewModel(t.FullName))).
                    Concat(directoryInfo.EnumerateDirectories()
                    .Select(d => new DirectoryViewModel(d.FullName)));
            }
            catch
            {
                Console.WriteLine("Ошибка при получении информации о директории");
                return Enumerable.Empty<object>();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
