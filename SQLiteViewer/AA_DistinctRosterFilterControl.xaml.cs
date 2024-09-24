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

namespace SQLiteViewer
{
    /// <summary>
    /// Interaction logic for AA_BetterReplaysUC.xaml
    /// </summary>
    public partial class AA_DistinctRosterFilterControl : UserControl
    {
        MainWindow mainWindow;
        public ObservableCollection<AA_DistinctRoster> FilteredData { get; set; }
        public ObservableCollection<AA_DistinctRoster> _pagedData { get; set; }
        private ObservableCollection<AA_DistinctRoster> OriginalData;
        private readonly DatabaseManager dbManager;

        private int _pageSize = 10;
        private int _currentPageIndex = 0;
        private int _totalPages;

        public AA_DistinctRosterFilterControl()
        {
            InitializeComponent();
            this.DataContext = this;

            // Initialize SQLitePCL.Batteries
            Batteries_V2.Init();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db");
            dbManager = new DatabaseManager(dbPath);

            // Initialize paged data collection
            OriginalData = dbManager.GetAllDistictRoaster();
            FilteredData = new ObservableCollection<AA_DistinctRoster>(OriginalData);
            _pagedData = new ObservableCollection<AA_DistinctRoster>();

            // Calculate total pages
            _totalPages = (OriginalData.Count + _pageSize - 1) / _pageSize;  // Rounded up division

            // Display the first page
            LoadPage(0);
        }

        // Method to load a specific page
        private void LoadPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _totalPages)
                return;

            _pagedData.Clear();
            var pageData = FilteredData.Skip(pageIndex * _pageSize).Take(_pageSize).ToList();

            foreach (var item in pageData)
            {
                _pagedData.Add(item);
            }

            DataGridView.ItemsSource = _pagedData;
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
            var filtered = OriginalData.AsEnumerable();

            //apply filters
            if (LevelFilter.IsSelected)
            {
                filtered = AA_DistinctRoster.FilterByLvlRange(filtered, Settings.Default.MinLvlTextBox, Settings.Default.MaxLvlTextBox);
            }
            if (PlatformFilter.IsSelected)
            {
                filtered = AA_DistinctRoster.FilterByPlatform(filtered, Settings.Default.PlatformComboBox);
            }
            if (PlaceFilter.IsSelected)
            {
                filtered = AA_DistinctRoster.FilterByPlaceRange(filtered, Settings.Default.MinPlaceTextBox, Settings.Default.MaxPlaceTextBox);
            }
            if (KillsFilter.IsSelected)
            {
                filtered = AA_DistinctRoster.FilterByKills(filtered, Settings.Default.MinKillsTextBox);
            }
            if (TeamFilter.IsSelected)
            {
                filtered = AA_DistinctRoster.FilterByTeam(filtered, Settings.Default.TeamTextBox);
            }

            FilteredData = new ObservableCollection<AA_DistinctRoster>(filtered);
            _totalPages = (FilteredData.Count + _pageSize - 1) / _pageSize;  // Rounded up division

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

        private void DateFilter_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void DateFilter_Unselected(object sender, RoutedEventArgs e)
        {

        }

        private void Kills_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Kills_Unselected(object sender, RoutedEventArgs e)
        {

        }

        private void PlaylistFilter_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void PlaylistFilter_Unselected(object sender, RoutedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await mainWindow.FilterDialogHost.ShowDialog(new AA_DistinctRosterDialog());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void DataGridView_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Skin")
            {
                e.Cancel = true;
            }
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
                FilteredData = new ObservableCollection<AA_DistinctRoster>(FilteredData.OrderBy(x => GetPropertyValue(x, sortBy)));
                e.Column.SortDirection = ListSortDirection.Descending;
            }
            else
            {
                FilteredData = new ObservableCollection<AA_DistinctRoster>(FilteredData.OrderByDescending(x => GetPropertyValue(x, sortBy)));
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
    }
}
