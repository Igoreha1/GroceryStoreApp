using GroceryStoreApp.AppData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace GroceryStoreApp
{
    public partial class MainStorePage : Page
    {
        private List<ProductCategories> _categories;
        private List<Products> _products;
        private int _currentCategoryId = 0;
        private string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroceryStoreDB;Integrated Security=True;";

        public MainStorePage()
        {
            InitializeComponent();
            Loaded += OnPageLoaded;
            UsernameTextBlock.Text = AppConnect.CurrentUser?.FirstName + " " + AppConnect.CurrentUser?.LastName;
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!TestDatabaseConnection())
                {
                    MessageBox.Show("Ошибка подключения к базе данных. Проверьте строку подключения.");
                    return;
                }

                LoadCategories();
                LoadProducts();

                Storyboard fadeIn = (Storyboard)FindResource("FadeIn");
                fadeIn.Begin(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}");
            }
        }

        private bool TestDatabaseConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД: {ex.Message}");
                return false;
            }
        }

        private void LoadCategories()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("SELECT CategoryID, CategoryName, Description FROM ProductCategories", connection);
                    using (var reader = command.ExecuteReader())
                    {
                        _categories = new List<ProductCategories>();
                        while (reader.Read())
                        {
                            _categories.Add(new ProductCategories
                            {
                                CategoryID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                CategoryName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                            });
                        }
                    }

                    var categoriesPanel = (StackPanel)AllProductsButton.Parent;

                    foreach (var category in _categories)
                    {
                        var btn = new Button
                        {
                            Content = category.CategoryName,
                            Tag = category.CategoryID,
                            Style = (Style)FindResource("PrimaryButtonStyle"),
                            Margin = new Thickness(5),
                            Padding = new Thickness(10, 5, 10, 5),
                            ToolTip = category.Description
                        };
                        btn.Click += CategoryButton_Click;
                        categoriesPanel.Children.Add(btn);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = _currentCategoryId == 0
                        ? "SELECT ProductID, ProductName, UnitPrice, CategoryID FROM Products"
                        : $"SELECT ProductID, ProductName, UnitPrice, CategoryID FROM Products WHERE CategoryID = {_currentCategoryId}";

                    var command = new SqlCommand(query, connection);
                    using (var reader = command.ExecuteReader())
                    {
                        _products = new List<Products>();
                        while (reader.Read())
                        {
                            try
                            {
                                _products.Add(new Products
                                {
                                    ProductID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                    ProductName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    UnitPrice = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                                    CategoryID = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                                });
                            }
                            catch (InvalidCastException ex)
                            {
                                MessageBox.Show($"Ошибка при чтении данных продукта: {ex.Message}");
                                continue;
                            }
                        }
                    }

                    DisplayProducts(_products);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продуктов: {ex.Message}\n\nДетали: {ex.InnerException?.Message}");
            }
        }

        private void DisplayProducts(List<Products> products)
        {
            ProductsWrapPanel.Children.Clear();

            if (!products.Any())
            {
                MessageBox.Show("Нет товаров в выбранной категории");
                return;
            }

            foreach (var product in products)
            {
                var productCard = CreateProductCard(product);
                ProductsWrapPanel.Children.Add(productCard);
            }
        }

        private Border CreateProductCard(Products product)
        {
            var border = new Border
            {
                Width = 200,
                Height = 250,
                Margin = new Thickness(10),
                CornerRadius = new CornerRadius(8),
                Background = Brushes.White,
                BorderBrush = (SolidColorBrush)FindResource("DividerBrush"),
                BorderThickness = new Thickness(1),
                Effect = new DropShadowEffect
                {
                    BlurRadius = 5,
                    ShadowDepth = 2,
                    Color = ((SolidColorBrush)FindResource("PrimaryBrush")).Color,
                    Opacity = 0.1
                }
            };

            var stackPanel = new StackPanel();

            var imageBorder = new Border
            {
                Height = 120,
                Background = (SolidColorBrush)FindResource("PrimaryLightBrush"),
                CornerRadius = new CornerRadius(8, 8, 0, 0)
            };

            var nameText = new TextBlock
            {
                Text = product.ProductName,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(10, 10, 10, 0),
                TextWrapping = TextWrapping.Wrap,
                Foreground = (SolidColorBrush)FindResource("TextBrush")
            };

            var priceText = new TextBlock
            {
                Text = $"{product.UnitPrice} руб.",
                Margin = new Thickness(10, 0, 10, 0),
                Foreground = (SolidColorBrush)FindResource("PrimaryDarkBrush")
            };

            var addButton = new Button
            {
                Content = "В корзину",
                Margin = new Thickness(10),
                Width = 120,
                Style = (Style)FindResource("PrimaryButtonStyle"),
                Tag = product.ProductID
            };
            addButton.Click += AddToCartButton_Click;

            stackPanel.Children.Add(imageBorder);
            stackPanel.Children.Add(nameText);
            stackPanel.Children.Add(priceText);
            stackPanel.Children.Add(addButton);

            border.Child = stackPanel;
            return border;
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Tag.ToString() == "0")
                {
                    _currentCategoryId = 0;
                }
                else if (button.Tag is int categoryId)
                {
                    _currentCategoryId = categoryId;
                }

                var categoriesPanel = (StackPanel)button.Parent;
                foreach (var child in categoriesPanel.Children)
                {
                    if (child is Button btn)
                    {
                        if (btn.Tag is int btnCategoryId && btnCategoryId == _currentCategoryId)
                        {
                            btn.Style = (Style)FindResource("ActiveCategoryButtonStyle");
                        }
                        else
                        {
                            btn.Style = (Style)FindResource("PrimaryButtonStyle");
                        }
                    }
                }

                LoadProducts();
            }
        }


        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int productId)
            {
                var product = _products.FirstOrDefault(p => p.ProductID == productId);
                if (product != null)
                {
                    MessageBox.Show($"{product.ProductName} добавлен в корзину!");
                }
            }
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

    public class ProductCategories
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    public class Products
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int CategoryID { get; set; }
    }
}