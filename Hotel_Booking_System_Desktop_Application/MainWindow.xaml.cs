using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Project;
using desktopapp;

namespace Project
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve input values
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Configure DbContextOptions
            var options = new DbContextOptionsBuilder<DataDbContext>()
                .UseSqlite(@"Data Source=D:\Projects\Hotel_Booking_System_Desktop_Application\product_app.db")
                .Options;

            // Check if the user exists in the database
            using (var context = new DataDbContext(options))
            {
                // Find the user by username
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    // Hash the input password
                    string hashedInputPassword = HashPassword(password);

                    // Compare the hashed input password with the stored password
                    if (hashedInputPassword == user.Password)
                    {
                        // Login successful
                        MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Navigate to the home page
                        Home home = new Home();
                        home.Show();
                        this.Close();
                    }
                    else
                    {
                        // Invalid password
                        MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // User not found
                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Open the Create Account window
            CreateAccountWindow account = new CreateAccountWindow();
            account.Show();
            this.Close();
        }

        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            // Open the Admin Login window
            AdminLoginWindow adminlogin = new AdminLoginWindow();
            adminlogin.Show();
            this.Close();
        }

        // Method to hash the password using SHA256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password string to a byte array
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convert the hash to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // "x2" formats each byte as a 2-digit hexadecimal number
                }

                return builder.ToString();
            }
        }
    }
}