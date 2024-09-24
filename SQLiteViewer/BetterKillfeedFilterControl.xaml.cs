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
using System.Security.AccessControl;
using System.ComponentModel;

namespace SQLiteViewer
{
    /// <summary>
    /// Interaction logic for AA_BetterReplaysUC.xaml
    /// </summary>
    public partial class BetterKillfeedFilterControl : UserControl
    {
        MainWindow mainWindow;
        public ObservableCollection<BetterKillfeed> FilteredData { get; set; }
        public ObservableCollection<BetterKillfeed> _pagedData { get; set; }
        private ObservableCollection<BetterKillfeed> OriginalData;
        private readonly DatabaseManager dbManager;

        private int _pageSize = 10;
        private int _currentPageIndex = 0;
        private int _totalPages;

        public BetterKillfeedFilterControl()
        {
            InitializeComponent();
            this.DataContext = this;

            // Initialize SQLitePCL.Batteries
            Batteries_V2.Init();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db");
            dbManager = new DatabaseManager(dbPath);

            // Initialize paged data collection
            OriginalData = dbManager.GetAllBetterKillfeed();
            FilteredData = new ObservableCollection<BetterKillfeed>(OriginalData);
            _pagedData = new ObservableCollection<BetterKillfeed>();

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
            if (ActionNumberFilter.IsSelected)
            {
                filtered = BetterKillfeed.FilterByActionNumber(filtered, Settings.Default.MinActionNumberTextBox, Settings.Default.MaxActionNumberTextBox);
            }
            if (StatusFilter.IsSelected)
            {
                filtered = BetterKillfeed.FilterByStatus(filtered, Settings.Default.StatusComboBox);
            }
            if (RarityFilter.IsSelected)
            {
                filtered = BetterKillfeed.FilterByRarity(filtered, Settings.Default.RarityComboBox);
            }
            if (WeaponFilter.IsSelected)
            {
                filtered = BetterKillfeed.FilterByWeapon(filtered, Settings.Default.WeaponTextBox);
            }
            if (POIFilter.IsSelected)
            {
                filtered = BetterKillfeed.FilterByPOI(filtered, Settings.Default.POITextBox);
            }

            FilteredData = new ObservableCollection<BetterKillfeed>(filtered);
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
            await mainWindow.FilterDialogHost.ShowDialog(new BetterKillfeedDialog());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void DataGridView_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
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
                FilteredData = new ObservableCollection<BetterKillfeed>(FilteredData.OrderBy(x => GetPropertyValue(x, sortBy)));
                e.Column.SortDirection = ListSortDirection.Descending;
            }
            else
            {
                FilteredData = new ObservableCollection<BetterKillfeed>(FilteredData.OrderByDescending(x => GetPropertyValue(x, sortBy)));
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
