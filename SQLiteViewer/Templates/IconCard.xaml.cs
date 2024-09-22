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

namespace SQLiteViewer.Templates
{
    /// <summary>
    /// Interaction logic for IconCard.xaml
    /// </summary>
    public partial class IconCard : UserControl
    {
        public ImageSource? ImageSource { get; set; }
        public event EventHandler? Navigate;
        public IconCard()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Navigate?.Invoke(this, EventArgs.Empty);

        }
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(IconCard), new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as IconCard;
            var isselected = (bool)e.NewValue;
            if (isselected)
            {
                control.BorderIcon.Background = (Brush)Application.Current.FindResource("IconBackground");
            }
            else
            {
                control.BorderIcon.Background = Brushes.Transparent;
            }
        }
    }
}
