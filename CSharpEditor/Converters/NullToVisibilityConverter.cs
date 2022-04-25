using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CSharpEditor.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        #region Properties

        public Visibility NullValue { get; set; } = Visibility.Collapsed;

        public Visibility NotNullValue { get; set; } = Visibility.Visible;
        #endregion Properties

        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            value == null ? NullValue : (object)NotNullValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotImplementedException();
        #endregion Methods
    }
}
