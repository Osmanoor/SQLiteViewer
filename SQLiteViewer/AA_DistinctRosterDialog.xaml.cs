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
    public partial class AA_DistinctRosterDialog : UserControl
    {
        public AA_DistinctRosterDialog()
        {
            InitializeComponent();
            var playlist = new List<string>(["PS5", "PSN", "WIN", "XBL", "XSX"]);
            MinLvlTextBox.Text = Settings.Default.MinLvlTextBox.ToString();
            MaxLvlTextBox.Text = Settings.Default.MaxLvlTextBox.ToString();
            MinPlaceTextBox.Text = Settings.Default.MinPlaceTextBox.ToString();
            MaxPlaceTextBox.Text = Settings.Default.MaxPlaceTextBox.ToString();
            PlatformComboBox.ItemsSource = playlist;
            PlatformComboBox.SelectedValue = Settings.Default.PlatformComboBox;
            MinKillsTextBox.Text = Settings.Default.MinKillsTextBox.ToString();
            TeamTextBox.Text = Settings.Default.TeamTextBox.ToString();
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.MinLvlTextBox = Convert.ToInt32(MinLvlTextBox.Text);
            Settings.Default.MaxLvlTextBox = Convert.ToInt32(MaxLvlTextBox.Text);
            Settings.Default.MinPlaceTextBox = Convert.ToInt32(MinPlaceTextBox.Text);
            Settings.Default.MaxPlaceTextBox = Convert.ToInt32(MaxPlaceTextBox.Text);
            Settings.Default.MinKillsTextBox = Convert.ToInt32(MinKillsTextBox.Text);
            Settings.Default.TeamTextBox = Convert.ToInt32(TeamTextBox.Text);
            Settings.Default.PlatformComboBox = (String)PlatformComboBox.SelectedValue??"PS5";
            Settings.Default.Save();
            DialogHost.CloseDialogCommand.Execute(true, null);
        }
    }
}
