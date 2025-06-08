using GroceryStoreApp.AppData;
using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GroceryStoreApp
{
    public partial class LoginPage : Page
    {
        private bool isPasswordVisible = false;

        public LoginPage()
        {
            InitializeComponent();
            Loaded += OnPageLoaded;

            AppConnect.model01 = new GroceryStoreDBEntities();
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            Storyboard fadeIn = (Storyboard)FindResource("FadeIn");
            fadeIn.Begin(LoginPanel);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка на пустые поля
                if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) ||
                   string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    ShowError("Введите логин и пароль");
                    return;
                }

                var userObj = AppConnect.model01.Users
                    .FirstOrDefault(x => x.Username == UsernameTextBox.Text &&
                                        x.Password == PasswordBox.Password);

                if (userObj == null)
                {
                    ShowError("Неверный логин или пароль");
                }
                else
                {
                    AppConnect.CurrentUser = userObj;
                    MessageBox.Show($"Добро пожаловать, {userObj.FirstName}!", "Успешный вход",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    StartFadeOutAnimation(() => NavigationService.Navigate(new MainStorePage()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;

            Storyboard shake = (Storyboard)FindResource("ShakeAnimation");
            Storyboard.SetTarget(shake, ErrorBorder);
            shake.Begin();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            StartFadeOutAnimation(() => NavigationService.Navigate(new RegisterPage()));
        }

        private void StartFadeOutAnimation(Action navigationAction)
        {
            Storyboard fadeOut = (Storyboard)FindResource("FadeOut");
            fadeOut.Completed += (s, args) => navigationAction();
            fadeOut.Begin(LoginPanel);
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            if (isPasswordVisible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
                TogglePasswordIcon.Text = "🙈";
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                TogglePasswordIcon.Text = "👁";
            }

            if (isPasswordVisible)
                PasswordTextBox.Focus();
            else
                PasswordBox.Focus();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var window = Window.GetWindow(this);

                window.DragMove();
            }
        }
    }
}