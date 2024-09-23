using System;
using System.Collections.Generic;
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
using SQLiteViewer.Properties;

namespace SQLiteViewer
{
    /// <summary>
    /// Interaction logic for BetterReplaysDialog.xaml
    /// </summary>
    public partial class BetterReplaysDialog : UserControl
    {
        public BetterReplaysDialog()
        {
            InitializeComponent();
            var playlist = new List<string>(["Solo","Duo"]);
            datePickerFrom.SelectedDate = Settings.Default.BetterReplayDateFrom;
            datePickerTo.SelectedDate = Settings.Default.BetterReplayDateTo;
            KillsFrom.Text = Settings.Default.BetterReplayKillsFrom.ToString();
            KillsTo.Text = Settings.Default.BetterReplayKillsTo.ToString();
            SeasonText.Text = Settings.Default.BetterReplaySeason.ToString();
            PlacementText.Text = Settings.Default.BetterReplayPlacement.ToString();
            comboBox.ItemsSource = playlist;
            comboBox.SelectedValue = Settings.Default.BetterReplayPlaylist;
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.BetterReplayDateFrom = datePickerFrom.SelectedDate??DateTime.Today;
            Settings.Default.BetterReplayDateTo = datePickerTo.SelectedDate??DateTime.Today.AddDays(1);
            Settings.Default.BetterReplayKillsFrom =Convert.ToInt32(KillsFrom.Text);
            Settings.Default.BetterReplayKillsTo =Convert.ToInt32(KillsTo.Text);
            Settings.Default.BetterReplaySeason = double.Parse(SeasonText.Text);
            Settings.Default.BetterReplayPlacement = int.Parse(PlacementText.Text);
            Settings.Default.BetterReplayPlaylist = (String)comboBox.SelectedValue??"Solo";
            Settings.Default.Save();
            DialogHost.CloseDialogCommand.Execute(true, null);
        }
    }
}
