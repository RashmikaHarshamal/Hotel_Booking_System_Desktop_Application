using Hotel_Booking_System_Desktop_Application;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project
{
    public partial class Booked : Window
    {
        public Booked()
        {
            InitializeComponent();
            LoadConfirmedRooms(); // Load confirmed rooms when the window is initialized
        }

        // Load confirmed rooms from the database
        private void LoadConfirmedRooms()
        {
            using (var context = new DataDbContext(GetDbContextOptions()))
            {
                // Fetch data from the Updates table where the room is confirmed
                var confirmedRooms = context.Updates.Where(r => r.IsConfirmed == true).ToList();

                // Convert room data to RoomViewModel for display
                var roomList = confirmedRooms.Select(room => new RoomViewModel
                {
                    Id = room.Id,
                    ImageSource = room.ProfileImagePath != null ? LoadImage(room.ProfileImagePath) : null,
                    NumberOfBeds = room.NumberOfBeds ?? 0,
                    Price = room.Price ?? 0.0m,
                    Type = room.Type
                }).ToList();

                // Bind the data to the DataGrid
                ConfirmedRoomsDataGrid.ItemsSource = roomList;
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
                var editForm = new Addform(roomId); // Use the Addform for editing
                editForm.ShowDialog();
                LoadConfirmedRooms(); // Refresh the DataGrid after editing
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
                            LoadConfirmedRooms(); // Refresh the DataGrid
                        }
                    }
                }
            }
        }

        // Event handler for the Dashboard button
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            AdminHomepage adminHomepage = new AdminHomepage();
            adminHomepage.Show();
            this.Close();
        }

        // Event handler for the Booked button
        private void Booked_Click(object sender, RoutedEventArgs e)
        {
            // Already on the Booked page, no action needed
        }

        // Event handler for the Available button
        private void Available_Click(object sender, RoutedEventArgs e)
        {
            Available availablePage = new Available();
            availablePage.Show();
            this.Close();
        }

        // Event handler for the Add Form button
        private void AddForm_Click(object sender, RoutedEventArgs e)
        {
            Addform addForm = new Addform();
            addForm.Show();
            this.Close();
        }

        // Event handler for the Logout button
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}