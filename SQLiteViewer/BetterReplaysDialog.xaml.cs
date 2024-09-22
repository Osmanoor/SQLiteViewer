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
            datePicker.SelectedDate = Settings.Default.BetterReplayDate;
            slider.Value = Settings.Default.BetterReplayKills;
            comboBox.ItemsSource = playlist;
            comboBox.SelectedValue = Settings.Default.BetterReplayPlaylist;
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.BetterReplayDate = datePicker.SelectedDate??DateTime.Today;
            Settings.Default.BetterReplayKills =Convert.ToInt32(slider.Value);
            Settings.Default.BetterReplayPlaylist = (String)comboBox.SelectedValue??"Solo";
            Settings.Default.Save();
            DialogHost.CloseDialogCommand.Execute(true, null);
        }
    }
}
