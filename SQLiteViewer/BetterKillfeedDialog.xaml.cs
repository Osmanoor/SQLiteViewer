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
    public partial class BetterKillfeedDialog : UserControl
    {
        public BetterKillfeedDialog()
        {
            InitializeComponent();
            var playlist1 = new List<string>(["Knocked", "Killed", "Revived"]);
            var playlist2 = new List<string>(["Common", "Uncommon", "Rare", "Epic", "Mythic", "Legendary"]);
            MinActionNumberTextBox.Text = Settings.Default.MinActionNumberTextBox.ToString();
            MaxActionNumberTextBox.Text = Settings.Default.MaxActionNumberTextBox.ToString();
            StatusComboBox.ItemsSource = playlist1;
            StatusComboBox.SelectedValue = Settings.Default.StatusComboBox;
            RarityComboBox.ItemsSource = playlist2;
            RarityComboBox.SelectedValue = Settings.Default.RarityComboBox;
            WeaponTextBox.Text = Settings.Default.WeaponTextBox.ToString();
            POITextBox.Text = Settings.Default.POITextBox.ToString();
        }

        private void Savebtn_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.MinActionNumberTextBox = Convert.ToInt32(MinActionNumberTextBox.Text);
            Settings.Default.MaxActionNumberTextBox = Convert.ToInt32(MaxActionNumberTextBox.Text);
            Settings.Default.StatusComboBox = (String)StatusComboBox.SelectedValue?? "Knocked";
            Settings.Default.RarityComboBox = (String)RarityComboBox.SelectedValue?? "Common";
            Settings.Default.WeaponTextBox = WeaponTextBox.Text;
            Settings.Default.POITextBox = POITextBox.Text;
            Settings.Default.Save();
            DialogHost.CloseDialogCommand.Execute(true, null);
        }
    }
}
