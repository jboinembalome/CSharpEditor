﻿namespace UnitComboboxDemo
{
  using System.Windows;
  using UnitComboboxDemo.ViewModels;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      DataContext = new AppViewModel();
    }
  }
}
