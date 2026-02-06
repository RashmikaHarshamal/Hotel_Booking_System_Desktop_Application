using Hotel_Booking_System_Desktop_Application;
using Project;
using System.Windows;

namespace desktopapp
{
    public partial class Home : Window
    {
        public Home() => InitializeComponent();

        private void Booking_Click(object sender, RoutedEventArgs e)
        {
            Booking book = new Booking();
            book.Show();
            this.Close();
        }

        private void MyAppointment_Click(object sender, RoutedEventArgs e)
        {
            MyAppointment myAppointmentWindow = new MyAppointment();
            myAppointmentWindow.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }


    }
}