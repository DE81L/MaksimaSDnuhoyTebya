using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;

namespace MaksimaSDnuhoyTebya
{
    public partial class MainWindow : Window
    {
        private List<PasswordEntry> passwordEntries;
        private readonly PasswordService passwordService;
        private const string FilePath = "passwords.json";

        public MainWindow()
        {
            InitializeComponent();
            passwordService = new PasswordService(FilePath);
            passwordEntries = passwordService.LoadPasswords() ?? new List<PasswordEntry>();
            UpdatePasswordList();
        }

        private void AddPassword_Click(object sender, RoutedEventArgs e)
        {
            var site = SiteTextBox.Text;
            var username = UsernameTextBox.Text;
            var password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(site) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполни все поля");
                return;
            }

            var encryptedPassword = EncryptionHelper.Encrypt(password);

            var passwordEntry = new PasswordEntry
            {
                Site = site,
                Username = username,
                EncryptedPassword = encryptedPassword
            };

            passwordEntries.Add(passwordEntry);
            passwordService.SavePasswords(passwordEntries);

            UpdatePasswordList();
            ClearInputFields();
        }

        private void UpdatePasswordList()
        {
            PasswordListBox.Items.Clear();
            foreach (var entry in passwordEntries)
            {
                PasswordListBox.Items.Add($"{entry.Site} - {entry.Username} - {EncryptionHelper.Decrypt(entry.EncryptedPassword)}");
            }
        }

        private void ClearInputFields()
        {
            SiteTextBox.Clear();
            UsernameTextBox.Clear();
            PasswordTextBox.Clear();
        }

        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordListBox.SelectedItem != null)
            {
                var selectedEntry = passwordEntries[PasswordListBox.SelectedIndex];
                var decryptedPassword = EncryptionHelper.Decrypt(selectedEntry.EncryptedPassword);
                Clipboard.SetText(decryptedPassword);
                MessageBox.Show("Скопировано.");
            }
        }

        private void EditPassword_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordListBox.SelectedItem != null)
            {
                var selectedEntry = passwordEntries[PasswordListBox.SelectedIndex];
                SiteTextBox.Text = selectedEntry.Site;
                UsernameTextBox.Text = selectedEntry.Username;
                PasswordTextBox.Password = EncryptionHelper.Decrypt(selectedEntry.EncryptedPassword);

                passwordEntries.RemoveAt(PasswordListBox.SelectedIndex);
                passwordService.SavePasswords(passwordEntries);
                UpdatePasswordList();
            }
        }

        private void DeletePassword_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordListBox.SelectedItem != null)
            {
                passwordEntries.RemoveAt(PasswordListBox.SelectedIndex);
                passwordService.SavePasswords(passwordEntries);
                UpdatePasswordList();
            }
        }

        private void PasswordListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = PasswordListBox.SelectedItem != null;
            CopyPasswordButton.IsEnabled = isItemSelected;
            EditButton.IsEnabled = isItemSelected;
            DeleteButton.IsEnabled = isItemSelected;
        }

        private void CopySite_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(SiteTextBox.Text);
            MessageBox.Show("Скопировано.");
        }

        private void CopyUsername_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(UsernameTextBox.Text);
            MessageBox.Show("Скопировано.");
        }
    }
}
