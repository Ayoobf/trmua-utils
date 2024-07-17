using Microsoft.Win32;
using System.Configuration;
using System.Windows;

namespace trmua_utils
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (Properties.Settings.Default.ThumbsFolderPath != null)
            {
                txtFolderPath.Text = Properties.Settings.Default.ThumbsFolderPath;
            }
        }
        private void SaveFolderPath(string folderPath)
        {
            try
            {
                Properties.Settings.Default.ThumbsFolderPath = folderPath;
                Properties.Settings.Default.Save();
                txtFolderPath.Text = folderPath;
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                SaveFolderPath(dialog.FolderName);
            }
        }

    }
}