using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MaterialDesignThemes.Wpf;
using SQLitePCL;
using System.IO;
using SQLiteViewer.Properties;
using System.ComponentModel;
using static MaterialDesignThemes.Wpf.Theme;

namespace SQLiteViewer
{
    /// <summary>
    /// Interaction logic for AA_BetterReplaysUC.xaml
    /// </summary>
    public partial class AA_BetterReplaysUC : UserControl
    {
        MainWindow mainWindow;
        public ObservableCollection<AA_BetterReplays> Replays { get; set; }
        public ObservableCollection<AA_BetterReplays> _pagedDataCollection { get; set; }
        private ObservableCollection<AA_BetterReplays> originalReplays;
        private readonly DatabaseManager dbManager;

        private int _pageSize = 10;
        private int _currentPageIndex = 0;
        private int _totalPages;

        public AA_BetterReplaysUC()
        {
            InitializeComponent();
            this.DataContext = this;

            // Initialize SQLitePCL.Batteries
            Batteries_V2.Init();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db");
            dbManager = new DatabaseManager(dbPath);

            // Initialize paged data collection
            originalReplays = dbManager.GetAllReplays();
            Replays = new ObservableCollection<AA_BetterReplays>(originalReplays);
            _pagedDataCollection = new ObservableCollection<AA_BetterReplays>();

            // Calculate total pages
            _totalPages = (originalReplays.Count + _pageSize - 1) / _pageSize;  // Rounded up division

            // Display the first page
            LoadPage(0);
        }

        // Method to load a specific page
        private void LoadPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _totalPages)
                return;

            _pagedDataCollection.Clear();
            var pageData = Replays.Skip(pageIndex * _pageSize).Take(_pageSize).ToList();

            foreach (var item in pageData)
            {
                _pagedDataCollection.Add(item);
            }

            DataGridView.ItemsSource = _pagedDataCollection;
            _currentPageIndex = pageIndex;

            // Update the page number display
            PageNumberTextBox.Text = (_currentPageIndex + 1).ToString();
        }
        // Event handler for "First" button
        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(0);
        }

        // Event handler for "Previous" button
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex > 0)
            {
                LoadPage(_currentPageIndex - 1);
            }
        }

        // Event handler for "Next" button
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex < _totalPages - 1)
            {
                LoadPage(_currentPageIndex + 1);
            }
        }

        // Event handler for "Last" button
        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(_totalPages - 1);
        }


        // Method to apply the filter and refresh the DataGrid
        private void ApplyFilters()
        {
            var filteredReplays = originalReplays.AsEnumerable();

            // Date Range Filter
            if (DateFilter.IsSelected)
            {
                filteredReplays = filteredReplays.Where(r => r.ReplayDate >= Settings.Default.BetterReplayDateFrom && r.ReplayDate <= Settings.Default.BetterReplayDateTo);
            }

            // Playlist Filter
            if (PlaylistFilter.IsSelected)
            {
                filteredReplays = filteredReplays.Where(r => r.Playlist == Settings.Default.BetterReplayPlaylist);
            }

            // Kills Range Filter
            if (Kills.IsSelected)
            {
                filteredReplays = filteredReplays.Where(r => r.Kills >= Settings.Default.BetterReplayKillsFrom && r.Kills <= Settings.Default.BetterReplayKillsTo);
            }

            // Season Filter
            if (SeasonFilter.IsSelected)
            {
                filteredReplays = filteredReplays.Where(r => r.Season == Settings.Default.BetterReplaySeason);
            }

            // Placement Filter
            if (PlacementFilter.IsSelected)
            {
                filteredReplays = filteredReplays.Where(r => r.Placement == Settings.Default.BetterReplayPlacement);
            }

            Replays = new ObservableCollection<AA_BetterReplays>(filteredReplays);
            LoadPage(0);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ApplyFilters();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await mainWindow.FilterDialogHost.ShowDialog(new BetterReplaysDialog());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void DataGridView_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true; // Cancel default sorting

            // Get the property name to sort by
            string sortBy = e.Column.SortMemberPath;

            // Check current sort direction
            ListSortDirection direction = e.Column.SortDirection ?? ListSortDirection.Ascending;

            // Perform the custom sorting on the full data set
            if (direction == ListSortDirection.Ascending)
            {
                Replays = new ObservableCollection<AA_BetterReplays>(Replays.OrderBy(x => GetPropertyValue(x, sortBy)));
                e.Column.SortDirection = ListSortDirection.Descending;
            }
            else
            {
                Replays = new ObservableCollection<AA_BetterReplays>(Replays.OrderByDescending(x => GetPropertyValue(x, sortBy)));
                e.Column.SortDirection = ListSortDirection.Ascending;
            }

            // Load the current page after sorting
            LoadPage(0);
        }

        // Helper method to get the property value using reflection
        private object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        private void DataGridView_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DataGridView.SelectedCells.Count > 0)
            {
                var selectedCellInfo = e.AddedCells[0];
                var columnHeader = selectedCellInfo.Column.Header.ToString();
                var selectedCell = DataGridView.SelectedCells[0];
                var cellInfo = selectedCell.Item as AA_BetterReplays;  // Replace with your data type

                if (cellInfo != null)
                {
                    // Check which column was selected and apply the respective filter
                    switch (selectedCell.Column.Header.ToString())
                    {
                        case "Playlist":
                            ApplyPlaylistFilter(cellInfo.Playlist);  // Assuming Playlist is a string
                            break;

                        case "Teammates":
                            ApplyTeammatesFilter(cellInfo.Teammates);  // Assuming Teammates is a string
                            break;

                        case "FileName":
                            ApplyFileNameFilter(cellInfo.FileName);  // Assuming FileName is a string
                            break;

                        case "ReplayDate":
                            ApplyReplayDateFilter(cellInfo.ReplayDate);  // Assuming ReplayDate is a DateTime
                            break;
                    }
                }
            }
        }
        // Filter by Playlist
        private void ApplyPlaylistFilter(string playlistValue)
        {
            Settings.Default.BetterReplayPlaylist = playlistValue;
            Settings.Default.Save();
            PlaylistFilter.Visibility = Visibility.Visible;
            PlaylistFilter.IsSelected = true;
            ApplyFilters();
        }

        // Filter by Teammates
        private void ApplyTeammatesFilter(string teammatesValue)
        {
            
        }

        // Filter by FileName
        private void ApplyFileNameFilter(string fileNameValue)
        {
            
        }

        // Filter by ReplayDate
        private void ApplyReplayDateFilter(DateTime replayDateValue)
        {
            
        }
    }
}
