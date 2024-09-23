using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SQLiteViewer
{
    public partial class BetterKillfeedFilterControl : UserControl
    {
        public ObservableCollection<BetterKillfeed> OriginalData { get; set; }
        public ObservableCollection<BetterKillfeed> FilteredData { get; set; }

        public event EventHandler<ObservableCollection<BetterKillfeed>> DataFiltered;

        public BetterKillfeedFilterControl()
        {
            InitializeComponent();
        }

        private void ApplyActionNumberFilter_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MinActionNumberTextBox.Text, out int minActionNumber) && int.TryParse(MaxActionNumberTextBox.Text, out int maxActionNumber))
            {
                FilteredData = BetterKillfeed.FilterByActionNumber(OriginalData, minActionNumber, maxActionNumber);
                OnDataFiltered();
            }
        }

        private void ApplyStatusFilter_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string status = selectedItem.Content.ToString();
                FilteredData = BetterKillfeed.FilterByStatus(OriginalData, status);
                OnDataFiltered();
            }
        }

        private void ApplyRarityFilter_Click(object sender, RoutedEventArgs e)
        {
            if (RarityComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string rarity = selectedItem.Content.ToString();
                FilteredData = BetterKillfeed.FilterByRarity(OriginalData, rarity);
                OnDataFiltered();
            }
        }

        private void ApplyWeaponFilter_Click(object sender, RoutedEventArgs e)
        {
            string weapon = WeaponTextBox.Text;
            FilteredData = BetterKillfeed.FilterByWeapon(OriginalData, weapon);
            OnDataFiltered();
        }

        private void ApplyPOIFilter_Click(object sender, RoutedEventArgs e)
        {
            string poi = POITextBox.Text;
            FilteredData = BetterKillfeed.FilterByPOI(OriginalData, poi);
            OnDataFiltered();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            FilteredData = new ObservableCollection<BetterKillfeed>(OriginalData);
            OnDataFiltered();
        }

        protected virtual void OnDataFiltered()
        {
            DataFiltered?.Invoke(this, FilteredData);
        }
    }
}