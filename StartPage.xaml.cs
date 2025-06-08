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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GroceryStoreApp
{
    public partial class StartPage : Page
    {
        private DispatcherTimer _timer;

        public StartPage()
        {
            InitializeComponent();
            Loaded += StartPage_Loaded;
        }

        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard logoAnimation = (Storyboard)FindResource("LogoAnimation");
            logoAnimation.Begin(LogoBorder);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            Storyboard fadeOut = (Storyboard)FindResource("FadeOutAnimation");
            fadeOut.Completed += FadeOut_Completed;
            fadeOut.Begin(this);
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}