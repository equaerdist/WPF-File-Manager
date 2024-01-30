using System;
using System.Globalization;
using System.IO;

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
            return directoryInfo.GetDirectories();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
