using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using RoslynEditor.Core.Completion;

namespace RoslynEditor.Core
{
    [ValueConversion(typeof(Glyph), typeof(ImageSource))]
    public class GlyphToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            (value as Glyph?)?.ToImageSource() ?? Binding.DoNothing;

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotSupportedException();
    }
}