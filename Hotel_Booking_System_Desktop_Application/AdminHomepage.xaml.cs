using Hotel_Booking_System_Desktop_Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project
{
    public partial class AdminHomepage : Window
    {
        public AdminHomepage()
        {
            InitializeComponent();
            LoadBookedRooms(); // Load booked room data when the window is initialized
        }

        // Load booked room data from the database
        private void LoadBookedRooms()
        {
            using (var context = new DataDbContext(GetDbContextOptions()))
            {
                // Fetch data from the Updates table where the room is booked
                var bookedRooms = context.Updates.Where(r => r.IsBooked == true && !r.IsConfirmed).ToList();

                // Convert room data to RoomViewModel for display
                var roomList = bookedRooms.Select(room => new RoomViewModel
                {
                    Id = room.Id,
                    ImageSource = room.ProfileImagePath != null ? LoadImage(room.ProfileImagePath) : null,
                    NumberOfBeds = room.NumberOfBeds ?? 0,
                    Price = room.Price ?? 0.0m,
                    Type = room.Type
                }).ToList();

                // Bind the data to the DataGrid
                BookedRoomsDataGrid.ItemsSource = roomList;
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

        // Event handler for the Confirm button
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int roomId)
            {
                using (var context = new DataDbContext(GetDbContextOptions()))
                {
                    var room = context.Updates.FirstOrDefault(r => r.Id == roomId);
                    if (room != null)
                    {
                        // Mark the room as confirmed
                        room.IsConfirmed = true; // Ensure this property exists in your Update entity
                        context.SaveChanges();

                        // Remove the room from the admin dashboard's list
                        var roomList = BookedRoomsDataGrid.ItemsSource as List<RoomViewModel>;
                        var roomToRemove = roomList?.FirstOrDefault(r => r.Id == roomId);
                        if (roomToRemove != null)
                        {
                            roomList.Remove(roomToRemove);
                            BookedRoomsDataGrid.ItemsSource = null; // Clear the current binding
                            BookedRoomsDataGrid.ItemsSource = roomList; // Rebind the updated list
                        }

                        MessageBox.Show("Room confirmed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        // Event handler for the Update button
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int roomId)
            {
                // Open the Update window with the selected room's details
                var updateForm = new Addform(roomId); // Use the Addform for updating
                updateForm.ShowDialog();
                LoadBookedRooms(); // Refresh the DataGrid after updating
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
                            LoadBookedRooms(); // Refresh the DataGrid
                        }
                    }
                }
            }
        }

        // Event handler for the Add Form button
        private void addform_Click(object sender, RoutedEventArgs e)
        {
            Addform form = new Addform();
            form.Show(); // Open the Add Form window
            this.Close(); // Close the current Admin Homepage window
        }

        // Event handler for the Available button
        private void available_Click(object sender, RoutedEventArgs e)
        {
            Available list = new Available();
            list.Show(); // Open the Available window
            this.Close(); // Close the current Admin Homepage window
        }

        // Event handler for the Booked button
        private void Booked_Click(object sender, RoutedEventArgs e)
        {
            Booked bookedPage = new Booked();
            bookedPage.Show(); // Open the Booked window
            this.Close(); // Close the current Admin Homepage window
        }

        // Event handler for the Logout button
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show(); // Open the Login window
            this.Close(); // Close the current Admin Homepage window
        }
    }
}