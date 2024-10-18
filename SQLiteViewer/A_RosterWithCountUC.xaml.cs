﻿using System;
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
using System.Diagnostics;

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

            public string ApplyOverlays(string basePath, string metK, string metD, int anon)
            {
                if (!File.Exists(basePath))
                {
                    Debug.WriteLine($"Base image not found: {basePath}");
                    return basePath;
                }

                try
                {
                    // Create a new unique filename for the overlay image
                    string outputPath = Path.Combine(
                        Path.GetDirectoryName(basePath),
                        Path.GetFileNameWithoutExtension(basePath) + "_overlay.png"
                    );

                    // Load the base image
                    BitmapImage baseImage = new BitmapImage();
                    baseImage.BeginInit();
                    baseImage.CacheOption = BitmapCacheOption.OnLoad;
                    baseImage.UriSource = new Uri(basePath);
                    baseImage.EndInit();

                    // Create the drawing visual
                    DrawingVisual drawingVisual = new DrawingVisual();
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        // Draw the base image
                        drawingContext.DrawImage(baseImage, new Rect(0, 0, baseImage.PixelWidth, baseImage.PixelHeight));

                        // Debug information
                        Debug.WriteLine($"Processing overlays for {Path.GetFileName(basePath)}");
                        Debug.WriteLine($"MetK: {metK}, MetD: {metD}, Anon: {anon}");

                        // Apply overlays based on conditions
                        if (metK == "1" && File.Exists(_overlayPaths["redX"]))
                        {
                            Debug.WriteLine("Applying redX overlay");
                            ApplyOverlay(drawingContext, _overlayPaths["redX"], baseImage.PixelWidth, baseImage.PixelHeight);
                        }
                        else if (metK == "2" && File.Exists(_overlayPaths["blueX"]))
                        {
                            Debug.WriteLine("Applying blueX overlay");
                            ApplyOverlay(drawingContext, _overlayPaths["blueX"], baseImage.PixelWidth, baseImage.PixelHeight);
                        }

                        if (metD == "1" && File.Exists(_overlayPaths["redGlow"]))
                        {
                            Debug.WriteLine("Applying redGlow overlay");
                            ApplyOverlay(drawingContext, _overlayPaths["redGlow"], baseImage.PixelWidth, baseImage.PixelHeight);
                        }
                        else if (metD == "2" && File.Exists(_overlayPaths["blueGlow"]))
                        {
                            Debug.WriteLine("Applying blueGlow overlay");
                            ApplyOverlay(drawingContext, _overlayPaths["blueGlow"], baseImage.PixelWidth, baseImage.PixelHeight);
                        }

                        if (anon == 1 && File.Exists(_overlayPaths["ghost"]))
                        {
                            Debug.WriteLine("Applying ghost overlay");
                            ApplyOverlay(drawingContext, _overlayPaths["ghost"], baseImage.PixelWidth, baseImage.PixelHeight);
                        }
                    }

                    // Create a render target bitmap
                    RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                        baseImage.PixelWidth,
                        baseImage.PixelHeight,
                        96, 96,
                        PixelFormats.Pbgra32);

                    renderBitmap.Render(drawingVisual);

                    // Save with PNG encoder
                    using (FileStream stream = File.Create(outputPath))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                        encoder.Save(stream);
                        stream.Flush();
                    }

                    Debug.WriteLine($"Saved overlay image to: {outputPath}");
                    return outputPath;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing image overlays: {ex.Message}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    return basePath;
                }
            }

            private void ApplyOverlay(DrawingContext drawingContext, string overlayPath, int width, int height)
            {
                try
                {
                    BitmapImage overlay = new BitmapImage();
                    overlay.BeginInit();
                    overlay.CacheOption = BitmapCacheOption.OnLoad;
                    overlay.UriSource = new Uri(overlayPath);
                    overlay.EndInit();

                    drawingContext.DrawImage(overlay, new Rect(0, 0, width, height));
                    Debug.WriteLine($"Successfully applied overlay: {Path.GetFileName(overlayPath)}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error applying overlay {overlayPath}: {ex.Message}");
                }
            }
        }

        private void LoadPage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= _totalPages)
                return;

            _pagedData.Clear();
            var pageData = dbManager.FilterAndPaginateRosterWithCount(
                fileName: _fileName,
                replayDate: _replayDate,
                displayName: _displayName,
                isBot: _isBot,
                isTeam: _isTeam,
                orderBy: _orderBy,
                ascending: _ascending,
                pageNumber: pageIndex,
                pageSize: _pageSize);

            // Create image processor instance
            var imageProcessor = new ImageOverlayProcessor();

            // Process each item and apply overlays
            foreach (var item in pageData)
            {
                Debug.WriteLine($"\nProcessing item: {item.DisplayName}");
                Debug.WriteLine($"Original Skin path: {item.Skin}");
                Debug.WriteLine($"MetK: {item.MetK}, MetD: {item.MetD}, Anon: {item.Anon}");

                // Apply overlays and set Skin2 property
                item.Skin2 = imageProcessor.ApplyOverlays(
                    item.Skin,
                    item.MetK,
                    item.MetD,
                    item.Anon
                );

                Debug.WriteLine($"Processed Skin2 path: {item.Skin2}");
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
                _isBot = IsBotFilter != null && IsBotFilter.IsSelected ? "1" : "";
                _isTeam = IsTeamFilter != null && IsTeamFilter.IsSelected ? "1" : "";
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
            if (e.PropertyName == "Skin" || e.PropertyName == "PlayerId" || e.PropertyName == "FileName")
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
