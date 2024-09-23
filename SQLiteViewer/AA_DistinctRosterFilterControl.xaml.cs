using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SQLiteViewer
{
    public partial class AA_DistinctRosterFilterControl : UserControl
    {
        public ObservableCollection<AA_DistinctRoster> OriginalData { get; set; }
        public ObservableCollection<AA_DistinctRoster> FilteredData { get; set; }

        public event EventHandler<ObservableCollection<AA_DistinctRoster>> DataFiltered;

        public AA_DistinctRosterFilterControl()
        {
            InitializeComponent();
        }

        private void ApplyLvlFilter_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.MinLvlTextBox.Text, out int minLvl) && int.TryParse(MaxLvlTextBox.Text, out int maxLvl))
            {
                FilteredData = AA_DistinctRoster.FilterByLvlRange(OriginalData, minLvl, maxLvl);
                OnDataFiltered();
            }
        }

        private void ApplyPlaceFilter_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MinPlaceTextBox.Text, out int minPlace) && int.TryParse(MaxPlaceTextBox.Text, out int maxPlace))
            {
                FilteredData = AA_DistinctRoster.FilterByPlaceRange(OriginalData, minPlace, maxPlace);
                OnDataFiltered();
            }
        }

        private void ApplyPlatformFilter_Click(object sender, RoutedEventArgs e)
        {
            if (PlatformComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string platform = selectedItem.Content.ToString();
                FilteredData = AA_DistinctRoster.FilterByPlatform(OriginalData, platform);
                OnDataFiltered();
            }
        }

        private void ApplyKillsFilter_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MinKillsTextBox.Text, out int minKills))
            {
                FilteredData = AA_DistinctRoster.FilterByKills(OriginalData, minKills);
                OnDataFiltered();
            }
        }

        private void ApplyTeamFilter_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TeamTextBox.Text, out int team))
            {
                FilteredData = AA_DistinctRoster.FilterByTeam(OriginalData, team);
                OnDataFiltered();
            }
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            FilteredData = new ObservableCollection<AA_DistinctRoster>(OriginalData);
            OnDataFiltered();
        }

        protected virtual void OnDataFiltered()
        {
            DataFiltered?.Invoke(this, FilteredData);
        }
    }
}