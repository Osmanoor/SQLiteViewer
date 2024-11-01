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
        private string _orderBy = "";
        private bool _ascending = true;
        private string _fileName = "";
        private string _actioner = "";
        private string _actionee = "";

        public BetterKillfeedFilterControl(string fileName="", string actioner="", string actionee ="")
        {
            InitializeComponent();
            this.DataContext = this;
            
            // Initialize SQLitePCL.Batteries
            Batteries_V2.Init();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db");
            dbManager = new DatabaseManager(dbPath);

            // Initialize paged data collection
            OriginalData = new ObservableCollection<BetterKillfeed>();
            FilteredData = new ObservableCollection<BetterKillfeed>(OriginalData);
            _pagedData = new ObservableCollection<BetterKillfeed>();

            // Calculate total pages
            _totalPages = (dbManager.GetRowCount("BetterKillfeed") + _pageSize - 1) / _pageSize;  // Rounded up division

           

            if (fileName != "")
            {
                ApplyFileNameFilter(fileName);
            }
            else
            {
                if (Settings.Default.FileNameFilter != "")
                {
                    ApplyFileNameFilter(Settings.Default.FileNameFilter);
                }
            }
            if (actioner != "")
            {
                ApplyActionerFilter(actioner);
            }
            else
            {
                if (Settings.Default.ActionerFilter != "" && actionee == "" && Settings.Default.FileNameFilter != "")
                {
                    ApplyActionerFilter(Settings.Default.ActionerFilter);
                }
            }
            if (actionee != "")
            {
                ApplyActioneeFilter(actionee);
            }
            else
            {
                if (Settings.Default.ActioneeFilter != "" && actioner == "" && Settings.Default.FileNameFilter != "")
                {
                    ApplyActioneeFilter(Settings.Default.ActioneeFilter);
                }
            }
            // Display the first page
            LoadPage(0);
        }

        // Method to load a specific page
        private void LoadPage(int pageIndex)
        {
            if (pageIndex < 0 )
                return;

            _pagedData.Clear();
            (var pageData,var total_rows) = dbManager.FilterAndPaginateBetterKillFeed(fileName: _fileName, actioner: _actioner, actionee: _actionee, orderBy: _orderBy, ascending: _ascending, pageNumber: pageIndex, pageSize: _pageSize);
            
            _totalPages = (total_rows + _pageSize - 1) / _pageSize;  // Rounded up division

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
                //ApplyFilters();
            }
            catch (Exception ex)
            {

                throw;
            }
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
            if (e.PropertyName == "FileName" || e.PropertyName == "ActionerId" || e.PropertyName == "ActioneeId")
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
            _orderBy = sortBy;
            // Perform the custom sorting on the full data set
            if (direction == ListSortDirection.Ascending)
            {
                _ascending = true;
                e.Column.SortDirection = ListSortDirection.Descending;
            }
            else
            {
                _ascending = false;
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
                var cellInfo = selectedCell.Item as BetterKillfeed;  // Replace with your data type

                if (cellInfo != null)
                {
                    // Check which column was selected and apply the respective filter
                    switch (selectedCell.Column.Header.ToString())
                    {
                        case "FileName":
                            ApplyFileNameFilter(cellInfo.FileName);  // Assuming Playlist is a string
                            LoadPage(0);
                            break;

                        case "ActionerId":
                            ApplyActionerFilter(cellInfo.ActionerId);  // Assuming Teammates is a string
                            LoadPage(0);
                            break;

                        case "ActioneeId":
                            ApplyActioneeFilter(cellInfo.ActioneeId);  // Assuming FileName is a string
                            LoadPage(0);
                            break;

                        case "ReplayDate":
                            //ApplyReplayDateFilter(cellInfo.ReplayDate);  // Assuming ReplayDate is a DateTime
                            break;
                    }
                }
            }
        }
        // Filter by Playlist
        private void ApplyFileNameFilter(string fileNameValue)
        {
            _fileName = fileNameValue;
            fileNameFilter.Visibility = Visibility.Visible;
            fileNameFilter.Content = $"FileName: {fileNameValue}";
            fileNameFilter.IsSelected = true;
            Settings.Default.FileNameFilter = fileNameValue;
            Settings.Default.Save();
        }
        private void ApplyActionerFilter(string actionerValue)
        {
            _actioner = actionerValue;
            ActionerFilter.Visibility = Visibility.Visible;
            ActionerFilter.Content =  $"ActionerID: {actionerValue}";
            ActionerFilter.IsSelected = true;
            Settings.Default.ActionerFilter = actionerValue;
            Settings.Default.Save();
        }
        private void ApplyActioneeFilter(string actioneeValue)
        {
            _actionee = actioneeValue;
            ActioneeFilter.Visibility = Visibility.Visible;
            ActioneeFilter.Content = $"ActioneeID: {actioneeValue}";
            ActioneeFilter.IsSelected = true;
            Settings.Default.ActioneeFilter = actioneeValue;
            Settings.Default.Save();
        }

        private void fileNameFilter_Unselected(object sender, RoutedEventArgs e)
        {
            fileNameFilter.Visibility = Visibility.Collapsed;
            _fileName = "";
            Settings.Default.FileNameFilter = "";
            Settings.Default.Save();
            LoadPage(0);
        }

        private void ActionerFilter_Unselected(object sender, RoutedEventArgs e)
        {
            ActionerFilter.Visibility = Visibility.Collapsed;
            _actioner = "";
            Settings.Default.ActionerFilter = "";
            Settings.Default.Save();
            LoadPage(0);
        }

        private void ActioneeFilter_Unselected(object sender, RoutedEventArgs e)
        {
            ActioneeFilter.Visibility = Visibility.Collapsed;
            _actionee = "";
            Settings.Default.ActioneeFilter = "";
            Settings.Default.Save();
            LoadPage(0);
        }
    }
}
