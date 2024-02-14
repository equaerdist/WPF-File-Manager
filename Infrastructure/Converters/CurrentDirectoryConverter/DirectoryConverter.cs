using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace fileChanger.Infrastructure.Converters.CurrentDirectoryConverter
{
    class DirectoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var directoryInfo = value as DirectoryInfo;
            if(directoryInfo is null)
                throw new ArgumentNullException(nameof(directoryInfo));
            try
            {
                var directories = directoryInfo.GetDirectories();
                return directories;
            }
            catch
            {
                Console.WriteLine("Ошибка прав");
                return Enumerable.Empty<DirectoryInfo>();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
