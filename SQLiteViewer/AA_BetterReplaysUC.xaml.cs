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
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace SQLiteViewer
{
    /// <summary>
    /// Interaction logic for AA_BetterReplaysUC.xaml
    /// </summary>
    public partial class AA_BetterReplaysUC : UserControl
    {
        MainWindow mainWindow;
        private ObservableCollection<YourDataModel> _originalDataCollection;
        private ObservableCollection<YourDataModel> _pagedDataCollection;

        private int _pageSize = 5;
        private int _currentPageIndex = 0;
        private int _totalPages;

        public AA_BetterReplaysUC()
        {
            InitializeComponent();
            this.DataContext = this;
            // Initialize the data
            refill();
            // Calculate total pages
            _totalPages = (_originalDataCollection.Count + _pageSize - 1) / _pageSize;  // Rounded up division

            // Initialize paged data collection
            _pagedDataCollection = new ObservableCollection<YourDataModel>();

            // Display the first page
            LoadPage(0);
        }

        // Method to load a specific page
        private void LoadPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _totalPages)
                return;

            _pagedDataCollection.Clear();
            var pageData = _originalDataCollection.Skip(pageIndex * _pageSize).Take(_pageSize).ToList();

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
        private void ApplyFilter(List<YourDataModel> filteredData)
        {
            _pagedDataCollection.Clear();
            foreach (var item in filteredData)
            {
                _pagedDataCollection.Add(item);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                refill();
                if (DateFilter.IsSelected)
                {
                    _originalDataCollection = new ObservableCollection<YourDataModel>(_originalDataCollection.Intersect(_originalDataCollection.Where(c => c.StringField == "Test1")));
                }
                if (Kills.IsSelected)
                {
                    _originalDataCollection = new ObservableCollection<YourDataModel>(_originalDataCollection.Intersect(_originalDataCollection.Where(c => c.IntField == 2)));
                }
                if (PlaylistFilter.IsSelected)
                {
                    _originalDataCollection = new ObservableCollection<YourDataModel>(_originalDataCollection.Intersect(_originalDataCollection.Where(c => c.BoolField == true)));
                }
                LoadPage(0);
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

        private void refill()
        {
            _originalDataCollection = new ObservableCollection<YourDataModel>
            {
                new YourDataModel { StringField = "Test1", IntField = 1, BoolField = true, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AbstractMirror_Rogue.png" },
                new YourDataModel { StringField = "Test2", IntField = 2, BoolField = false, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AccentWall.png" },
                new YourDataModel { StringField = "Test1", IntField = 1, BoolField = true, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AbstractMirror_Rogue.png" },
                new YourDataModel { StringField = "Test2", IntField = 2, BoolField = false, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AccentWall.png" },
                new YourDataModel { StringField = "Test1", IntField = 1, BoolField = true, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AbstractMirror_Rogue.png" },
                new YourDataModel { StringField = "Test2", IntField = 2, BoolField = false, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AccentWall.png" },
                new YourDataModel { StringField = "Test1", IntField = 1, BoolField = true, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AbstractMirror_Rogue.png" },
                new YourDataModel { StringField = "Test2", IntField = 2, BoolField = false, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AccentWall.png" },
                new YourDataModel { StringField = "Test1", IntField = 1, BoolField = true, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AbstractMirror_Rogue.png" },
                new YourDataModel { StringField = "Test2", IntField = 2, BoolField = false, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AccentWall.png" },
                new YourDataModel { StringField = "Test1", IntField = 1, BoolField = true, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AbstractMirror_Rogue.png" },
                new YourDataModel { StringField = "Test2", IntField = 2, BoolField = false, ImageField = "D:\\Projects\\Desktop Development\\SQLiteViewer\\SQLiteViewer\\Resources\\Character_AccentWall.png" },
            };
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await mainWindow.FilterDialogHost.ShowDialog(new BetterReplaysDialog());
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }
    }

    public class YourDataModel
    {
        public string StringField { get; set; }
        public int IntField { get; set; }
        public bool BoolField { get; set; }
        public string ImageField { get; set; }  // Path to the image
    }
}
