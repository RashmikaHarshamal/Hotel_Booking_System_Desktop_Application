using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using desktopapp;
using Microsoft.EntityFrameworkCore;

namespace Project
{
    public partial class MyAppointment : Window
    {
        public MyAppointment()
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
                var bookedRooms = context.Updates.Where(r => r.IsBooked == true).ToList();

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

        // Event handler for the Home button
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Home homeWindow = new Home();
            homeWindow.Show(); // Open the Home window
            this.Close();       // Close the current MyAppointment window
        }

        // Event handler for the Booking button
        private void Booking_Click(object sender, RoutedEventArgs e)
        {
            Booking bookingWindow = new Booking();
            bookingWindow.Show(); // Open the Booking window
            this.Close();          // Close the current MyAppointment window
        }

        // Event handler for the Logout button
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show(); // Open the Login window
            this.Close();       // Close the current MyAppointment window
        }
    }
}