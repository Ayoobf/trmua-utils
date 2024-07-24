using Microsoft.Win32;
using System.Configuration;
using System.Windows;
using trmua_utils.utils;
using MessageBox = System.Windows.MessageBox;

namespace trmua_utils
{

    public partial class MainWindow : Window
    {
        private RotateFile _rotateFile;
        private RemoveThumbs _removeThumbs;
        private Refresh _refresh;
        private bool _isRemovingThumbs = false;
        private bool _isRotating = false;
        private bool _isRefreshing = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            _rotateFile = new RotateFile();
            _removeThumbs = new RemoveThumbs();
            _refresh = new Refresh();
            _rotateFile.LogMessage += LogMessage;
            _removeThumbs.LogMessage += LogMessage;
            _refresh.LogMessage += LogMessage;
            UpdateStopButtonState();
            Top = 500;
            Left = 1100;
        }

        // Loads the settings from memory
        private void LoadSettings()
        {
            if (Properties.Settings.Default.ThumbsFolderPath != null)
            {
                thumbsFolderPath.Text = Properties.Settings.Default.ThumbsFolderPath;
                rotateFolderPath.Text = Properties.Settings.Default.RotateFolderPath;
            }
        }

        // Saves folder path needed in order for "remove thumbs" to work
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

        // Button behavior for Saving the folder path on the second page of the application 
        private void ThumbsFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                SaveFolderPathThumbs(dialog.FolderName);
            }
        }

        // Saves folder path needed in order for "rotate folder" to work
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

        // Button behavior for Saving the folder path on the second page of the application 
        private void RotateFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                SaveFolderPathRotate(dialog.FolderName);
            }
        }

        // Button behavior for running the rotate file button
        private async void RotateFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rotateFolderPath.Text))
            {
                MessageBox.Show("Please Select a folder first. ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            rotate.IsEnabled = false;
            _isRotating = true;
            UpdateStopButtonState();
            var progress = new Progress<int>(value =>
            {
                // Update progress bar if you add one
            });

            try
            {
                await _rotateFile.RotateFilesAsync(rotateFolderPath.Text, 2, progress, Dispatcher);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                rotate.IsEnabled = true;
                _isRotating = false;
                UpdateStopButtonState();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isRotating)
                {
                    _rotateFile.Stop();
                    _isRotating = false;
                }
                if (_isRemovingThumbs)
                {
                    _removeThumbs.Stop();
                    removeThumbs.Content = "Remove Thumbs";
                    _isRemovingThumbs = false;
                }
                if (_isRefreshing)
                {
                    _refresh.Stop();
                    _isRefreshing = false;
                }
                UpdateStopButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper method for pushing log messages
        private void LogMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                outputTextBlock.Text += message + Environment.NewLine;
            });
        }

        // Button behavior for running the "remove thumbs util"
        private void RemoveThumbs_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(thumbsFolderPath.Text))
            {
                MessageBox.Show("Please select a folder first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_isRemovingThumbs)
            {
                _removeThumbs.Stop();
                removeThumbs.Content = "Remove Thumbs";
                _isRemovingThumbs = false;
            }
            else
            {
                _removeThumbs.StartRemovingThumbs(thumbsFolderPath.Text, Dispatcher);
                removeThumbs.Content = "Stop Removing Thumbs";
                _isRemovingThumbs = true;
            }
            UpdateStopButtonState();
        }

        private void UpdateStopButtonState()
        {
            Stop.IsEnabled = _isRotating || _isRemovingThumbs || _isRefreshing;
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                refresh.IsEnabled = false;
                _isRefreshing = true;
                UpdateStopButtonState();
                await _refresh.RefreshAsync("HPSCANNER", Dispatcher);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                refresh.IsEnabled = true;
                _isRefreshing = false;
                UpdateStopButtonState();
            }

        }

        private void Window_PreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            var window = (Window)sender;
            window.Topmost = true;
        }
    }
}