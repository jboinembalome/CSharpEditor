using System;
using System.Windows;

namespace RoslynEditor.Windows
{
    public static class CommonEvent
    {
        public static RoutedEvent Register<TOwner, TEventArgs>(string name, RoutingStrategy routing) 
            where TEventArgs : RoutedEventArgs => 
            EventManager.RegisterRoutedEvent(name, routing, typeof(EventHandler<TEventArgs>), typeof(TOwner));
    }
}