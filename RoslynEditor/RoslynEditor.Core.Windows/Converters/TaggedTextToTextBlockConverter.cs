﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core
{
    [ValueConversion(typeof(IEnumerable<TaggedText>), typeof(TextBlock))]
    public sealed class TaggedTextToTextBlockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            (value as IEnumerable<TaggedText>)?.ToTextBlock() ?? Binding.DoNothing;

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            throw new NotSupportedException();
    }
}