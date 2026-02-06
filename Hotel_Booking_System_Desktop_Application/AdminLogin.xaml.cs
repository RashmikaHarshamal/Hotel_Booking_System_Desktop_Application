using Hotel_Booking_System_Desktop_Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Project
{
    public partial class AdminLoginWindow : Window
    {
        public AdminLoginWindow() => InitializeComponent();

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowErrorPopup("Please enter both username and password.");
                return;
            }

            try
            {
                // Configure DbContextOptions
                var options = new DbContextOptionsBuilder<DataDbContext>()
                    .UseSqlite(@"Data Source=D:\Projects\Hotel_Booking_System_Desktop_Application\product_app.db")
                    .Options;

                using (var context = new DataDbContext(options))
                {
                    // Check if database is connected
                    if (!context.Database.CanConnect())
                    {
                        ShowErrorPopup("Database connection failed.");
                        return;
                    }

                    // Retrieve admin from the database
                    var admin = context.Admindata
                        .FirstOrDefault(a => a.Username.ToLower() == username.ToLower());

                    if (admin == null)
                    {
                        ShowErrorPopup("Admin user not found.");
                        return;
                    }

                    // Check if password matches
                    if (admin.Password == password)
                    {
                        // Login successful
                        MessageBox.Show("Admin login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Navigate to the admin homepage
                        AdminHomepage adminHomepage = new AdminHomepage();
                        adminHomepage.Show();
                        this.Close();
                    }
                    else
                    {
                        ShowErrorPopup("Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorPopup("Error during login: " + ex.Message);
            }
        }

        private void UserLogin_Click(object sender, RoutedEventArgs e)
        {
            // Open the user login window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ShowErrorPopup(string errorMessage)
        {
            // Create a popup window for errors
            Window popupWindow = new Window
            {
                Width = 300,
                Height = 230,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None, // Remove title bar and close button
                Background = Brushes.White,
                Topmost = true
            };

            // StackPanel for content
            StackPanel stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Error message
            TextBlock errorTextBlock = new TextBlock
            {
                Text = errorMessage,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Black
            };

            // OK button
            Button okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Height = 30,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Color.FromRgb(210, 77, 255)), // Light purple button (#d24dff)
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            // Close popup on button click
            okButton.Click += (s, args) => popupWindow.Close();

            // Add elements
            stackPanel.Children.Add(errorTextBlock);
            stackPanel.Children.Add(okButton);
            popupWindow.Content = stackPanel;
            popupWindow.ShowDialog();
        }
    }
}