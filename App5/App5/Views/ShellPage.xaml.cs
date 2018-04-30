using System;

using App5.ViewModels;

using Windows.UI.Xaml.Controls;

namespace App5.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView);
        }
    }
}
