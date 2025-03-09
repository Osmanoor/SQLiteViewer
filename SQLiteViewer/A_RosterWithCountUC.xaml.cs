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
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;


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

        private bool first = true;

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

            
            
            // Initialize SQLitePCL.Batteries
            Batteries_V2.Init();
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db");
            dbManager = new DatabaseManager(dbPath);

            // Initialize paged data collection
            OriginalData = new ObservableCollection<A_RosterWithCount>();
            FilteredData = new ObservableCollection<A_RosterWithCount>();
            _pagedData = new ObservableCollection<A_RosterWithCount>();

            // Calculate total pages
            _totalPages = ((dbManager.GetRowCount("A_RosterWithCount") + _pageSize - 1) / _pageSize);  // Rounded up division

            

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
            if (replayDate != null)
            {
                ApplyReplayDateFilter(replayDate.Value);
            }
            else
            {
                if (Settings.Default.ReplayDateFilter.Date.Year != 1)
                {
                    ApplyReplayDateFilter(Settings.Default.ReplayDateFilter);
                }
            }
            if (Settings.Default.DisplayNameFilter != "")
            {
                ApplyDisplayNameFilter(Settings.Default.DisplayNameFilter);
            }

            // Display the first page
          //  LoadPage(0);
        }

        // Method to load a specific page
        private class ImageOverlayProcessor
        {
            private readonly Dictionary<string, string> _overlayPaths;

            public ImageOverlayProcessor()
            {
                string overlayDirectory = Path.Combine(Environment.CurrentDirectory, "Overlays");
                _overlayPaths = new Dictionary<string, string>
            {
                { "redX", Path.Combine(overlayDirectory, "redX.png") },
                { "blueX", Path.Combine(overlayDirectory, "blueX.png") },
                { "redGlow", Path.Combine(overlayDirectory, "redglow.png") },
                { "blueGlow", Path.Combine(overlayDirectory, "blueglow.png") },
                { "ghost", Path.Combine(overlayDirectory, "ghost.png") }
            };
            }
        }

        private void LoadPage(int pageIndex)
        {
            if (pageIndex < 0 )
                return;

            _pagedData.Clear();
            (var pageData, var total_rows) = dbManager.FilterAndPaginateRosterWithCount(fileName: _fileName, replayDate: _replayDate, displayName: _displayName, isBot:_isBot, isTeam:_isTeam,isAnon:_isAnon, orderBy: _orderBy, ascending: _ascending, pageNumber: pageIndex , pageSize: _pageSize);

            // Calculate total pages
            _totalPages = ((total_rows + _pageSize - 1) / _pageSize);  // Rounded up division

            foreach (var item in pageData)
            {
                List<Bitmap> bitmaps = new List<Bitmap>();
                if (File.Exists(item.Skin))
                {
                    bitmaps.Add(new Bitmap(item.Skin));
                }
                if (item.MetD == "1")
                {
                    bitmaps.Add( new Bitmap(LoadBitmapFromByteArray(Properties.Resources.redglow)));
                }
                if (item.MetD == "2")
                {
                    bitmaps.Add(new Bitmap(LoadBitmapFromByteArray(Properties.Resources.glow)));
                }
                if (item.MetK == "1")
                {
                    bitmaps.Add(new Bitmap(LoadBitmapFromByteArray(Properties.Resources.redx)));
                }
                if (item.MetK == "2")
                {
                    bitmaps.Add(new Bitmap(LoadBitmapFromByteArray(Properties.Resources.bluex)));
                }
                if (item.Anon == 1)
                {
                    bitmaps.Add(new Bitmap(LoadBitmapFromByteArray(Properties.Resources.ghost)));
                }
                Bitmap result = Merge(bitmaps);
                item.BitmapSource = ConvertBitmapToBitmapSource(result);
                _pagedData.Add(item);
            }

            DataGridView.ItemsSource = _pagedData;
            _currentPageIndex = pageIndex;
            PageNumberTextBox.Text = (_currentPageIndex + 1).ToString();
        }

        // Helper method to apply specific overlays based on conditions
        private void ApplyOverlay(DrawingContext dc, string condition, string overlay1, string overlay2, BitmapImage baseImage)
        {
            if (condition == "1")
                DrawOverlay(dc, overlay1, baseImage);
            else if (condition == "2")
                DrawOverlay(dc, overlay2, baseImage);
        }

        // Draws the overlay image at the top-left corner
        private void DrawOverlay(DrawingContext dc, string overlayFileName, BitmapImage baseImage)
        {
            string overlayPath = Path.Combine(Environment.CurrentDirectory, "Overlays", overlayFileName);
            if (File.Exists(overlayPath))
            {
                BitmapImage overlayImage = new BitmapImage(new Uri(overlayPath));
                dc.DrawImage(overlayImage, new Rect(0, 0, baseImage.Width, baseImage.Height));  // Draw overlay
            }
        }

        // Method to save the final image to a file
        private void SaveBitmap(RenderTargetBitmap bitmap, string filePath)
        {
            // Use a memory stream to avoid file locking issues
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                BitmapEncoder encoder = new PngBitmapEncoder(); // You can use any encoder (PNG, JPEG, etc.)
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
            }
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
                if (first)
                {
                    return;
                }
                _isBot = IsBotFilter != null && IsBotFilter.IsSelected ? "1" : "";
                _isTeam = IsTeamFilter != null && IsTeamFilter.IsSelected ? "1" : "";
                _isAnon = IsAnonFilter != null && IsAnonFilter.IsSelected ? "1" : "";

                if (IsBotFilter != null) Settings.Default.IsBotFilter = IsBotFilter.IsSelected;
                if (IsTeamFilter != null) Settings.Default.IsTeamFilter = IsTeamFilter.IsSelected;
                if (IsAnonFilter != null) Settings.Default.IsAnonFilter = IsAnonFilter.IsSelected;
                Settings.Default.Save();
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
            mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            
            IsBotFilter.IsSelected = Settings.Default.IsBotFilter;
            _isBot = IsBotFilter.IsSelected ? "1" : "";
            IsTeamFilter.IsSelected = Settings.Default.IsTeamFilter;
            _isTeam = IsTeamFilter.IsSelected ? "1" : "";
            IsAnonFilter.IsSelected = Settings.Default.IsAnonFilter;
            _isAnon = IsAnonFilter.IsSelected ? "1" : "";
            first = false;
            LoadPage(0);
        }

        private void DataGridView_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Skin" || e.PropertyName == "PlayerId" || e.PropertyName == "FileName"
                || e.PropertyName == "MetK" || e.PropertyName == "MetD" || e.PropertyName == "Anon" || e.PropertyName == "BitmapSource")
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
                            LoadPage(0);
                            break;

                        case "FileName":
                            ApplyFileNameFilter(cellInfo.FileName);  // Assuming FileName is a string
                            LoadPage(0);
                            break;

                        case "Date":
                            ApplyReplayDateFilter(cellInfo.Date);  // Assuming ReplayDate is a DateTime
                            LoadPage(0);
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
            Settings.Default.FileNameFilter = fileNameValue;
            Settings.Default.Save();
            
        }
        private void ApplyReplayDateFilter(DateTime replayDateValue)
        {
            _replayDate = replayDateValue;
            ReplayDateFilter.Visibility = Visibility.Visible;
            ReplayDateFilter.Content = $"Date: {replayDateValue}";
            ReplayDateFilter.IsSelected = true;
            Settings.Default.ReplayDateFilter = replayDateValue;
            Settings.Default.Save();
        }
        private void ApplyDisplayNameFilter(string displayNameValue)
        {
            _displayName = displayNameValue;
            DisplayNameFilter.Visibility = Visibility.Visible;
            DisplayNameFilter.Content = $"PlayerID: {displayNameValue}";
            DisplayNameFilter.IsSelected = true;
            Settings.Default.DisplayNameFilter = displayNameValue;
            Settings.Default.Save();
        }
        private void FileNameFilter_Unselected(object sender, RoutedEventArgs e)
        {
            FileNameFilter.Visibility = Visibility.Collapsed;
            _fileName = "";
            Settings.Default.FileNameFilter = "";
            Settings.Default.Save();
            LoadPage(0);
        }

        private void ReplayDateFilter_Unselected(object sender, RoutedEventArgs e)
        {
            ReplayDateFilter.Visibility = Visibility.Collapsed;
            _replayDate = null;
            Settings.Default.ReplayDateFilter = new DateTime();
            Settings.Default.Save();
            LoadPage(0);
        }

        private void DisplayNameFilter_Unselected(object sender, RoutedEventArgs e)
        {
            DisplayNameFilter.Visibility = Visibility.Collapsed;
            _displayName = "";
            Settings.Default.DisplayNameFilter = "";
            Settings.Default.Save();
            LoadPage(0);
        }
        private static Bitmap Merge(List<Bitmap> images)
        {
            try
            {
                var enumerable = images as List<Bitmap> ?? images.ToList();
                var sample = enumerable.First();
                var bitmap = new Bitmap(sample.Width, sample.Height);
                bitmap.SetResolution(sample.HorizontalResolution, sample.VerticalResolution);
                using (var g = Graphics.FromImage(bitmap))
                {
                    foreach (var image in enumerable)
                    {
                        g.DrawImage(image, 0, 0);
                    }
                }
                //foreach (var bmp in enumerable)
                //{
                //    bmp.Dispose();
                //}
                return bitmap;
            }
            catch (Exception)
            {
                return new Bitmap(120, 120);
            }
            

        }
        public BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Save the bitmap to the memory stream
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Reset the stream position to the beginning
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Create a BitmapImage from the stream
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Ensure the stream can be closed after loading
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Make it cross-thread accessible (optional)

                return bitmapImage;
            }
        }
        public Bitmap LoadBitmapFromByteArray(byte[] imageData)
        {
            using (var memoryStream = new MemoryStream(imageData))
            {
                // Create a Bitmap from the memory stream
                return new Bitmap(memoryStream);
            }
        }

        private void IsBotFilter_Selected(object sender, RoutedEventArgs e)
        {
            if (first) return;
            _isBot = IsBotFilter != null && IsBotFilter.IsSelected ? "1" : "";


            if (IsBotFilter != null) Settings.Default.IsBotFilter = IsBotFilter.IsSelected;

            Settings.Default.Save();
            LoadPage(0);
        }

        private void IsTeamFilter_Selected(object sender, RoutedEventArgs e)
        {
            if (first) return;
            _isTeam = IsTeamFilter != null && IsTeamFilter.IsSelected ? "1" : "";

            if (IsTeamFilter != null) Settings.Default.IsTeamFilter = IsTeamFilter.IsSelected;
            Settings.Default.Save();
            LoadPage(0);
        }

        private void IsAnonFilter_Selected(object sender, RoutedEventArgs e)
        {
            if (first) return;
            _isAnon = IsAnonFilter != null && IsAnonFilter.IsSelected ? "1" : "";
            if (IsAnonFilter != null) Settings.Default.IsAnonFilter = IsAnonFilter.IsSelected;
            Settings.Default.Save();
            LoadPage(0);
        }

    }
}
