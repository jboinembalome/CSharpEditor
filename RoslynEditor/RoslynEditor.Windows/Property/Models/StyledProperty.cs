using System;
using System.Windows;

namespace RoslynEditor.Windows
{
    public sealed class StyledProperty<TValue>
    {
        public DependencyProperty Property { get; }

        public StyledProperty(DependencyProperty property) => Property = property;

        public StyledProperty<TValue> AddOwner<TOwner>() => new(Property.AddOwner(typeof(TOwner)));

        public Type PropertyType => Property.PropertyType;
    }
}
