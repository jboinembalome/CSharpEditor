﻿using System.Linq;
using System.Windows;

namespace RoslynEditor.Core
{
    internal static class WindowExtensions
    {
        public static void SetOwnerToActive(this Window window) => 
            window.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive);
    }
}