using GroceryStoreApp.AppData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GroceryStoreApp
{
    public partial class MainStorePage : Page
    {
        public MainStorePage()
        {
            InitializeComponent();
            Loaded += OnPageLoaded;

            UsernameTextBlock.Text = AppConnect.CurrentUser.FirstName + " " + AppConnect.CurrentUser.LastName;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            Storyboard fadeIn = (Storyboard)FindResource("FadeIn");
            fadeIn.Begin(this);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            fadeOut.Completed += (s, _) =>
            {
                NavigationService?.Navigate(new LoginPage());
            };

            this.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}