using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Homework_17.Models;

namespace Homework_17
{
    public partial class MainWindow : Window
    {
        private HomeworkDbContext context;
        private User selectedUser;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            await Task.Delay(100);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            context = new HomeworkDbContext();
            LoadUsers();
        }

        private void LoadUsers()
        {
            var users = context.Users.OrderBy(u => u.Id).ToList();
            dg1.ItemsSource = users;
        }

        private void LoadProducts()
        {
            if (selectedUser != null)
            {
                var products = context.Products.Where(g => g.Email == selectedUser.Email).OrderBy(g => g.Id).ToList();
                dg2.ItemsSource = products;
            }
        }

        private void dg1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedUser = (User)e.Row.Item;

            if (string.IsNullOrEmpty(editedUser.LastName) ||
                string.IsNullOrEmpty(editedUser.FirstName) ||
                string.IsNullOrEmpty(editedUser.MiddleName) ||
                string.IsNullOrEmpty(editedUser.Email) ||
                string.IsNullOrEmpty(editedUser.PhoneNumber))
            {
                e.Cancel = true;
            }
            else
            {
                context.SaveChanges();
            }
        }

        private void dg1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dg1.SelectedItem is User user)
            {
                selectedUser = user;
                LoadProducts();
            }
        }

        private void dg1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is DataGridCell && e.Key == Key.Delete)
            {
                if (MessageBox.Show($"Are you sure you want to delete client {selectedUser.Id}?", "Deleting client", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    context.Users.Remove(selectedUser);
                    context.SaveChanges();
                    selectedUser = null;
                    LoadProducts();
                }
            }
        }

        private void MenuItem_ClearDB(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the user database?", "Clearing the database", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                foreach (var user in context.Users)
                {
                    context.Users.Remove(user);
                }
                context.SaveChanges();

                selectedUser = null;
                LoadUsers();
                LoadProducts();
            }
        }
    }
}
