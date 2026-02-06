using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Microsoft.EntityFrameworkCore;

namespace Project
{
    public partial class Addform : Window
    {
        private int? _roomId; // Store the room ID for editing

        // Constructor for adding a new room
        public Addform() => InitializeComponent();

        // Constructor for editing an existing room
        public Addform(int roomId)
        {
            InitializeComponent();
            _roomId = roomId;

            // Load the room details for editing
            using (var context = new DataDbContext(GetDbContextOptions()))
            {
                var room = context.Updates.FirstOrDefault(r => r.Id == roomId);
                if (room != null)
                {
                    // Populate the form with existing data
                    txtNumberOfBeds.Text = room.NumberOfBeds.ToString();
                    txtPrice.Text = room.Price.ToString();
                    txtType.Text = room.Type;

                    // Load the image if it exists
                    if (room.ProfileImagePath != null)
                    {
                        RoomImagePreview.Source = LoadImage(room.ProfileImagePath);
                        RoomImagePreview.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        // Event handler for the Choose Image button
        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select an image
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                Title = "Select a Room Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Load the selected image into the Image control
                string imagePath = openFileDialog.FileName;
                RoomImagePreview.Source = new BitmapImage(new Uri(imagePath));
                RoomImagePreview.Visibility = Visibility.Visible;
            }
        }

        // Event handler for the Submit button
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve values from the form
            string numberOfBedsText = txtNumberOfBeds.Text;
            string priceText = txtPrice.Text;
            string type = txtType.Text;

            // Validate input fields
            if (string.IsNullOrWhiteSpace(numberOfBedsText) ||
                string.IsNullOrWhiteSpace(priceText) ||
                string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Parse numeric values
            if (!int.TryParse(numberOfBedsText, out int numberOfBeds))
            {
                MessageBox.Show("Invalid number of beds.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Invalid price.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Convert the selected image to a byte array
            byte[] profileImageBytes = null;
            if (RoomImagePreview.Source is BitmapImage bitmapImage)
            {
                string imagePath = bitmapImage.UriSource.LocalPath;
                profileImageBytes = File.ReadAllBytes(imagePath);
            }

            // Save to database
            using (var context = new DataDbContext(GetDbContextOptions()))
            {
                Update update;
                if (_roomId.HasValue)
                {
                    // Editing an existing room
                    update = context.Updates.FirstOrDefault(r => r.Id == _roomId);
                    if (update != null)
                    {
                        update.NumberOfBeds = numberOfBeds;
                        update.Price = price;
                        update.Type = type;
                        update.ProfileImagePath = profileImageBytes;
                    }
                }
                else
                {
                    // Adding a new room
                    update = new Update
                    {
                        NumberOfBeds = numberOfBeds,
                        Price = price,
                        Type = type,
                        ProfileImagePath = profileImageBytes
                    };
                    context.Updates.Add(update);
                }

                context.SaveChanges();
            }

            MessageBox.Show("Room details submitted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Navigate back to the Available window
            Available availableWindow = new Available();
            availableWindow.Show(); // Open the Available window
            this.Close(); // Close the current window
        }

        // Method to get DbContextOptions for DataDbContext
        private DbContextOptions<DataDbContext> GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseSqlite(@"Data Source=D:\Projects\Hotel_Booking_System_Desktop_Application\product_app.db");
            return optionsBuilder.Options;
        }

        // Event handler for the Available button
        private void available_Click(object sender, RoutedEventArgs e)
        {
            Available availableWindow = new Available();
            availableWindow.Show(); // Open the Available window
            this.Close(); // Close the current window
        }

        // Event handler for the Dashboard button
        private void homepage_Click(object sender, RoutedEventArgs e)
        {
            AdminHomepage adminhomepage = new AdminHomepage();
            adminhomepage.Show(); // Open the Admin Homepage window
            this.Close(); // Close the current window
        }

        // Event handler for the Logout button
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show(); // Open the Login window
            this.Close(); // Close the current window
        }

        // Convert byte array to BitmapImage
        private BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            var image = new BitmapImage();
            using (var stream = new System.IO.MemoryStream(imageData))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }
            return image;
        }
    }
}