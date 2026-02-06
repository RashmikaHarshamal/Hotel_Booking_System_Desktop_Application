using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;

namespace Project
{
    public partial class Available : Window
    {
        public Available()
        {
            InitializeComponent();
            LoadRoomData(); // Load room data when the window is initialized
        }

        // Load room data from the database
        private void LoadRoomData()
        {
            using (var context = new DataDbContext(GetDbContextOptions()))
            {
                // Fetch data from the Updates table
                var rooms = context.Updates.ToList();

                // Convert ProfileImagePath to BitmapImage for display
                var roomList = rooms.Select(room => new RoomViewModel
                {
                    Id = room.Id,
                    ImageSource = room.ProfileImagePath != null ? LoadImage(room.ProfileImagePath) : null,
                    NumberOfBeds = room.NumberOfBeds ?? 0, // Use 0 if NumberOfBeds is null
                    Price = room.Price ?? 0.0m, // Use 0.0 if Price is null
                    Type = room.Type
                }).ToList();

                // Bind the data to the DataGrid
                RoomDataGrid.ItemsSource = roomList;
            }
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

        // Method to get DbContextOptions for DataDbContext
        private DbContextOptions<DataDbContext> GetDbContextOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
            optionsBuilder.UseSqlite(@"Data Source=D:\Projects\Hotel_Booking_System_Desktop_Application\product_app.db");
            return optionsBuilder.Options;
        }

        // Event handler for the Edit button
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int roomId)
            {
                // Open the Edit window with the selected room's details
                var editForm = new Addform(roomId);
                editForm.ShowDialog();
                LoadRoomData(); // Refresh the DataGrid after editing
            }
        }

        // Event handler for the Delete button
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int roomId)
            {
                // Confirm deletion
                var result = MessageBox.Show("Are you sure you want to delete this room?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    // Delete the room from the database
                    using (var context = new DataDbContext(GetDbContextOptions()))
                    {
                        var room = context.Updates.FirstOrDefault(r => r.Id == roomId);
                        if (room != null)
                        {
                            context.Updates.Remove(room);
                            context.SaveChanges();
                            LoadRoomData(); // Refresh the DataGrid
                        }
                    }
                }
            }
        }

        // Event handler for the Add Form button
        private void addform_Click(object sender, RoutedEventArgs e)
        {
            var addForm = new Addform();
            addForm.ShowDialog();
            LoadRoomData(); // Refresh the DataGrid after adding
        }

        // Event handler for the Dashboard button
        private void homepage_Click(object sender, RoutedEventArgs e)
        {
            AdminHomepage adminhomepage = new AdminHomepage();
            adminhomepage.Show(); // Open the Admin Homepage window
            this.Close(); // Close the current window
        }

        // Event handler for the Available button
        private void available_Click(object sender, RoutedEventArgs e)
        {
            // Open the Available window
            Available availableWindow = new Available();
            availableWindow.Show(); // Open the Available window
            this.Close(); // Close the current window
        }

        // Event handler for the Logout button
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show(); // Open the Login window
            this.Close(); // Close the current window
        }

        // Event handler for the Booked button
        private void Booked_Click(object sender, RoutedEventArgs e)
        {
            Booked bookedWindow = new Booked();
            bookedWindow.Show(); // Open the Booked window
            this.Close(); // Close the current window
        }

    }

    // ViewModel for displaying room data in the DataGrid
    public class RoomViewModel
    {
        public int Id { get; set; }
        public BitmapImage ImageSource { get; set; }
        public int NumberOfBeds { get; set; } // Non-nullable int
        public decimal Price { get; set; } // Non-nullable decimal
        public string Type { get; set; }
    }
}