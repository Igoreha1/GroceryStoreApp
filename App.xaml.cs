using GroceryStoreApp.AppData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace GroceryStoreApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new NavigationWindow
            {
                Source = new Uri("StartPage.xaml", UriKind.Relative),
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.CanResize,
                ShowsNavigationUI = false
            };

            mainWindow.Show();
        }
    }
}