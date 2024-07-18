using Microsoft.Win32;
using System.Configuration;
using System.Windows;
using trmua_utils.utils;

namespace trmua_utils
{

    public partial class MainWindow : Window
    {
        private RotateFile _rotateFile;
        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            _rotateFile = new RotateFile();
            _rotateFile.LogMessage += LogMessage;
        }

      
        private void LoadSettings()
        {
            if (Properties.Settings.Default.ThumbsFolderPath != null)
            {
                thumbsFolderPath.Text = Properties.Settings.Default.ThumbsFolderPath;
                rotateFolderPath.Text = Properties.Settings.Default.RotateFolderPath;
            }
        }
        private void SaveFolderPathThumbs(string folderPath)
        {
            try
            {
                Properties.Settings.Default.ThumbsFolderPath = folderPath;
                Properties.Settings.Default.Save();
                thumbsFolderPath.Text = folderPath;
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ThumbsFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                SaveFolderPathThumbs(dialog.FolderName);
            }
        }

        private void SaveFolderPathRotate(string folderPath)
        {
            try
            {
                Properties.Settings.Default.RotateFolderPath = folderPath;
                Properties.Settings.Default.Save();
                rotateFolderPath.Text = folderPath;
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RotateFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                SaveFolderPathRotate(dialog.FolderName);
            }
        }

        private async void RotateFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rotateFolderPath.Text))
            {
                MessageBox.Show("Please Select a folder first. ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            rotate.IsEnabled = false;
            Stop.IsEnabled = true;
            var progress = new Progress<int>(value =>
            {
                // Update progress bar if you add one
            });

            try
            {
                await _rotateFile.RotateFilesAsync(rotateFolderPath.Text, 1, progress, Dispatcher);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                rotate.IsEnabled = true;
                Stop.IsEnabled = false;
            }
        }
        private void LogMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                outputTextBlock.Text += message + Environment.NewLine;
            });
        }
    }
}