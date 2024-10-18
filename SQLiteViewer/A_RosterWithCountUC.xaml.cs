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
    public partial class A_RosterWithCountUC : UserControl
    {
        MainWindow mainWindow;
        public ObservableCollection<A_RosterWithCount> FilteredData { get; set; }
        public ObservableCollection<A_RosterWithCount> _pagedData { get; set; }
        private ObservableCollection<A_RosterWithCount> OriginalData;
        private readonly DatabaseManager dbManager;

        private int _pageSize = 10;
        private int _currentPageIndex = 0;
        private int _totalPages;
        private string _orderBy = "";
        private bool _ascending = true;
        private string _fileName = "";
        private DateTime? _replayDate = null;
        private string _displayName = "";
        private string _isBot = "1";
        private string _isTeam = "1";
        private string _isAnon = "1";

        public A_RosterWithCountUC(string fileName = "",DateTime? replayDate=null)
        {
            InitializeComponent();
            this.DataContext = this;

            if (fileName != "")
            {
                ApplyFileNameFilter(fileName);
            }
            if (replayDate != null)
            {
                ApplyReplayDateFilter(replayDate.Value);
            }
            
            // Initialize SQLitePCL.Batteries
            Batteries_V2.Init();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db");
            dbManager = new DatabaseManager(dbPath);

            // Initialize paged data collection
            OriginalData = new ObservableCollection<A_RosterWithCount>();
            FilteredData = new ObservableCollection<A_RosterWithCount>(OriginalData);
            _pagedData = new ObservableCollection<A_RosterWithCount>();

            // Calculate total pages
            _totalPages = ((dbManager.GetRowCount("A_RosterWithCount") + _pageSize - 1) / _pageSize);  // Rounded up division

            // Display the first page
            LoadPage(0);
        }

        // Method to load a specific page
        private void LoadPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _totalPages)
                return;

            _pagedData.Clear();
            var pageData = dbManager.FilterAndPaginateRosterWithCount(fileName: _fileName, replayDate: _replayDate, displayName: _displayName, isBot:_isBot, isTeam:_isTeam,isAnon:_isAnon, orderBy: _orderBy, ascending: _ascending, pageNumber: pageIndex , pageSize: _pageSize);

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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _isBot = IsBotFilter != null && IsBotFilter.IsSelected ? "1" : "";
                _isTeam = IsTeamFilter != null && IsTeamFilter.IsSelected ? "1" : "";
                _isAnon = IsAnonFilter != null && IsAnonFilter.IsSelected ? "1" : "";
                LoadPage(0);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
         //   await mainWindow.FilterDialogHost.ShowDialog(new AA_DistinctRosterDialog());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void DataGridView_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Skin" || e.PropertyName == "PlayerId" || e.PropertyName == "FileName"
                || e.PropertyName == "MetK" || e.PropertyName == "MetD" || e.PropertyName == "Anon")
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
                var cellInfo = selectedCell.Item as A_RosterWithCount;  // Replace with your data type

                if (cellInfo != null)
                {
                    // Check which column was selected and apply the respective filter
                    switch (selectedCell.Column.Header.ToString())
                    {
                        case "DisplayName":
                            ApplyDisplayNameFilter(cellInfo.PlayerId);  // Assuming Teammates is a string
                            break;

                        case "FileName":
                            ApplyFileNameFilter(cellInfo.FileName);  // Assuming FileName is a string
                            break;

                        case "Date":
                            ApplyReplayDateFilter(cellInfo.Date);  // Assuming ReplayDate is a DateTime
                            break;

                        case "Kills":
                            mainWindow.UnSelectAll();
                            mainWindow.NavigateTo(new BetterKillfeedFilterControl(cellInfo.FileName,actioner: cellInfo.PlayerId));
                            break;

                        case "Place":
                            mainWindow.UnSelectAll();
                            mainWindow.NavigateTo(new BetterKillfeedFilterControl(cellInfo.FileName,actionee: cellInfo.PlayerId));
                            break;
                    }
                }
            }
        }
        // Filter by Playlist
        private void ApplyFileNameFilter(string fileNameValue)
        {
            _fileName = fileNameValue;
            FileNameFilter.Visibility = Visibility.Visible;
            FileNameFilter.Content = $"FileName: {fileNameValue}";
            FileNameFilter.IsSelected = true;
            LoadPage(0);
        }
        private void ApplyReplayDateFilter(DateTime replayDateValue)
        {
            _replayDate = replayDateValue;
            ReplayDateFilter.Visibility = Visibility.Visible;
            ReplayDateFilter.Content = $"Date: {replayDateValue}";
            ReplayDateFilter.IsSelected = true;
            LoadPage(0);
        }
        private void ApplyDisplayNameFilter(string displayNameValue)
        {
            _displayName = displayNameValue;
            DisplayNameFilter.Visibility = Visibility.Visible;
            DisplayNameFilter.Content = $"PlayerID: {displayNameValue}";
            DisplayNameFilter.IsSelected = true;
            LoadPage(0);
        }
        private void FileNameFilter_Unselected(object sender, RoutedEventArgs e)
        {
            FileNameFilter.Visibility = Visibility.Collapsed;
            _fileName = "";
            LoadPage(0);
        }

        private void ReplayDateFilter_Unselected(object sender, RoutedEventArgs e)
        {
            ReplayDateFilter.Visibility = Visibility.Collapsed;
            _replayDate = null;
            LoadPage(0);
        }

        private void DisplayNameFilter_Unselected(object sender, RoutedEventArgs e)
        {
            DisplayNameFilter.Visibility = Visibility.Collapsed;
            _displayName = "";
            LoadPage(0);
        }
    }
}
