using GroceryStoreApp.AppData;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace GroceryStoreApp
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Storyboard fadeIn = (Storyboard)FindResource("FadeIn");
            fadeIn.Begin(RegistrationPanel);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string username = RegisterUsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string phone = PhoneNumberTextBox.Text;
            string password = RegisterPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowError("Заполните все обязательные поля");
                return;
            }

            if (string.IsNullOrWhiteSpace(firstName) || firstName.Length < 2)
            {
                ShowError("Имя должно содержать минимум 2 буквы");
                return;
            }

            if (firstName.Any(char.IsDigit))
            {
                ShowError("Имя не должно содержать цифры");
                return;
            }

            if (string.IsNullOrWhiteSpace(lastName) || lastName.Length < 2)
            {
                ShowError("Фамилия должна содержать минимум 2 буквы");
                return;
            }

            if (lastName.Any(char.IsDigit))
            {
                ShowError("Фамилия не должна содержать цифры");
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || username.Length < 4 || !Regex.IsMatch(username, "[a-zA-Z]"))
            {
                ShowError("Логин должен содержать минимум 4 символа, включая английские буквы");
                return;
            }

            if (username.Contains(" "))
            {
                ShowError("Логин не должен содержать пробелов");
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                ShowError("Введите корректный email");
                return;
            }

            if (string.IsNullOrWhiteSpace(phone) || phone.Length < 10 || !phone.All(char.IsDigit))
            {
                ShowError("Введите корректный телефон (минимум 10 цифр)");
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ShowError("Пароль должен содержать минимум 6 символов");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Пароли не совпадают");
                return;
            }

            try
            {
                if (AppConnect.model01.Users.Any(u => u.Username == username))
                {
                    ShowError("Этот логин уже занят");
                    return;
                }

                if (AppConnect.model01.Users.Any(u => u.Email == email))
                {
                    ShowError("Этот email уже зарегистрирован");
                    return;
                }

                Users newUser = new Users
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Email = email,
                    PhoneNumber = phone,
                    Password = password,
                    RegistrationDate = DateTime.Now,
                };

                AppConnect.model01.Users.Add(newUser);
                AppConnect.model01.SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                StartFadeOutAnimation(() => NavigationService?.Navigate(new LoginPage()));
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка регистрации: {ex.Message}");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
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

        private void LoginLinkButton_Click(object sender, RoutedEventArgs e)
        {
            StartFadeOutAnimation(() => NavigationService?.Navigate(new LoginPage()));
        }

        private void StartFadeOutAnimation(Action navigationAction)
        {
            Storyboard fadeOut = (Storyboard)FindResource("FadeOut");
            fadeOut.Completed += (s, args) => navigationAction();
            fadeOut.Begin(RegistrationPanel);
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