using Hotel_Booking_System_Desktop_Application;
using Microsoft.EntityFrameworkCore;
using Project;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Project
{
    public partial class CreateAccountWindow : Window
    {
        public CreateAccountWindow() => InitializeComponent();

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve input values
            string username = txtUsername.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // Perform validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Hash the password using SHA256
            string hashedPassword = HashPassword(password);

            // Create a new user object
            var user = new User
            {
                Username = username,
                Email = email,
                Password = hashedPassword
            };

            // Configure DbContextOptions
            var options = new DbContextOptionsBuilder<DataDbContext>()
                .UseSqlite(@"Data Source=D:\Projects\Hotel_Booking_System_Desktop_Application\product_app.db")
                .Options;

            // Save the user to the database
            using (var context = new DataDbContext(options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Navigate to the login page
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Open the login window
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
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