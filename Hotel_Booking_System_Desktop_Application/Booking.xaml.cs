using desktopapp;
using Hotel_Booking_System_Desktop_Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project
{
    public partial class Booking : Window
    {
        public Booking()
        {
            InitializeComponent();
            LoadUpdatedRooms(); // Load updated room data when the window is initialized
        }

        // Load updated room data from the database
        private void LoadUpdatedRooms()
        {
            using (var context = new DataDbContext(GetDbContextOptions()))
            {
                var updatedRooms = context.Updates.ToList();

                var roomList = updatedRooms.Select(room => new RoomViewModel
                {
                    Id = room.Id,
                    ImageSource = room.ProfileImagePath != null ? LoadImage(room.ProfileImagePath) : null,
                    NumberOfBeds = room.NumberOfBeds ?? 0,
                    Price = room.Price ?? 0.0m,
                    Type = room.Type
                }).ToList();

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

        // Handle the "Book" button click
        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int roomId)
            {
                using (var context = new DataDbContext(GetDbContextOptions()))
                {
                    var room = context.Updates.FirstOrDefault(r => r.Id == roomId);
                    if (room != null)
                    {
                        // Mark the room as booked
                        room.IsBooked = true;
                        context.SaveChanges();
                        MessageBox.Show("Room booked successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Open the MyAppointment window
                        MyAppointment myAppointmentWindow = new MyAppointment();
                        myAppointmentWindow.Show(); // Open the My Appointment window
                        this.Close(); // Close the current Booking window
                    }
                }
            }
        }

        // Event handler for the Home button
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Home homeWindow = new Home();
            homeWindow.Show(); // Open the Home window
            this.Close();       // Close the current Booking window
        }

        // Event handler for the My Appointment button
        private void MyAppointment_Click(object sender, RoutedEventArgs e)
        {
            MyAppointment myAppointmentWindow = new MyAppointment();
            myAppointmentWindow.Show(); // Open the My Appointment window
            this.Close();               // Close the current Booking window
        }

        // Event handler for the Logout button
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show(); // Open the Login window
            this.Close();       // Close the current Booking window
        }
    }
}