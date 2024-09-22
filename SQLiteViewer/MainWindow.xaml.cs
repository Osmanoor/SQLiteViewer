using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

using MaterialDesignThemes.Wpf;

using SQLiteViewer.Properties;
using SQLiteViewer.Templates;
using static MaterialDesignThemes.Wpf.Theme;

namespace SQLiteViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public List<UserControl> Pages { get; set; }
        public Storyboard slideInStoryboard;
        public Storyboard slideInLeftStoryboard;
        public Storyboard slideInTopStoryboard;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                if (screenHeight < 1080)
                {
                    this.Height = 820;
                }
                if (screenHeight < 850)
                {
                    this.Height = 700;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"MainWindow Error: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                slideInStoryboard = (Storyboard)FindResource("SlideInFromRight");
                slideInLeftStoryboard = (Storyboard)FindResource("SlideInFromLeft");
                slideInTopStoryboard = (Storyboard)FindResource("SlideInFromTop");
                Pages = new List<UserControl>();
                BeginSlidingTop(HeaderGrid);
                BeginSlidingLeft(IconGrid);
                homeBtn.IsSelected = true;
                NavigateTo(new AA_BetterReplaysUC());
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
        public void BeginSliding(UserControl element)
        {
            if (slideInStoryboard != null)
            {
                element.RenderTransform = new TranslateTransform();
                slideInStoryboard.Begin(element);
            }
        }
        public void BeginSlidingLeft(Grid element)
        {
            if (slideInStoryboard != null)
            {
                element.RenderTransform = new TranslateTransform();
                slideInLeftStoryboard.Begin(element);
            }
        }
        public void BeginSlidingTop(Grid element)
        {
            if (slideInStoryboard != null)
            {
                element.RenderTransform = new TranslateTransform();
                slideInTopStoryboard.Begin(element);
            }
        }
        public void ShowError(string message)
        {
            MessageBox.Show(message);
        }
        private void UnSelectAll()
        {
            homeBtn.IsSelected = false;
            featureBtn.IsSelected = false;
            backupBtn.IsSelected = false;
            settingsBtn.IsSelected = false;

        }
        private void homeBtn_Navigate(object sender, EventArgs e)
        {
            try
            {
                UnSelectAll();
                var iconCard = (IconCard)sender;
                iconCard.IsSelected = true;
                var dashboard = new AA_BetterReplaysUC();

                NavigateTo(dashboard);
                scrollViewer.ScrollToTop();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

        }

        private void featureBtn_Navigate(object sender, EventArgs e)
        {
            try
            {
                UnSelectAll();
                var iconCard = (IconCard)sender;
                iconCard.IsSelected = true;
                NavigateTo(new AA_BetterReplaysUC());
                scrollViewer.ScrollToTop();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }


        }

        private void backupBtn_Navigate(object sender, EventArgs e)
        {
            try
            {
                UnSelectAll();
                var iconCard = (IconCard)sender;
                iconCard.IsSelected = true;
                NavigateTo(new AA_BetterReplaysUC());
                scrollViewer.ScrollToTop();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }


        }

        private void settingsBtn_Navigate(object sender, EventArgs e)
        {
            try
            {
                UnSelectAll();
                var iconCard = (IconCard)sender;
                iconCard.IsSelected = true;
                NavigateTo(new AA_BetterReplaysUC());
                scrollViewer.ScrollToTop();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }

        }

        public async void NavigateTo(UserControl newControl)
        {
            newControl.Style = FindResource("TransitioningUserControl") as Style;
            if (scrollViewer.Content != null)
            {
                var oldControl = scrollViewer.Content as UserControl;
                if (oldControl != null)
                {
                    var storyboard = new Storyboard();
                    var fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
                    fadeOutAnimation.Completed += (s, e) =>
                    {
                        scrollViewer.Content = newControl;
                    };
                    storyboard.Children.Add(fadeOutAnimation);
                    Storyboard.SetTarget(fadeOutAnimation, oldControl);
                    Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UserControl.OpacityProperty));
                    storyboard.Begin();
                    await Task.Delay(500);
                    scrollViewer.Content = newControl;
                    scrollViewer.ScrollToTop();
                }
            }
            else
            {
                scrollViewer.Content = newControl;
            }
        }

        private void DraggableGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                // Begin dragging the window
                this.DragMove();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MainBorder.CornerRadius = new CornerRadius(0);
            }
            else
            {
                MainBorder.CornerRadius = new CornerRadius(50);
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //############################################# Windows Resize #################################################
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hwnd)?.AddHook(new HwndSourceHook(WindowProc));
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOM = 15;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;
            const int WM_NCHITTEST = 0x0084;

            if (msg == WM_NCHITTEST)
            {
                POINT mousePos = GetMousePosition();
                Rect windowRect = new Rect(this.Left, this.Top, this.ActualWidth, this.ActualHeight);

                const int RESIZE_BORDER_THICKNESS = 10;

                if (mousePos.Y >= windowRect.Top && mousePos.Y < windowRect.Top + RESIZE_BORDER_THICKNESS)
                {
                    if (mousePos.X >= windowRect.Left && mousePos.X < windowRect.Left + RESIZE_BORDER_THICKNESS)
                    {
                        handled = true;
                        return (IntPtr)HTTOPLEFT;
                    }
                    if (mousePos.X >= windowRect.Right - RESIZE_BORDER_THICKNESS && mousePos.X < windowRect.Right)
                    {
                        handled = true;
                        return (IntPtr)HTTOPRIGHT;
                    }
                    handled = true;
                    return (IntPtr)HTTOP;
                }
                if (mousePos.Y >= windowRect.Bottom - RESIZE_BORDER_THICKNESS && mousePos.Y < windowRect.Bottom)
                {
                    if (mousePos.X >= windowRect.Left && mousePos.X < windowRect.Left + RESIZE_BORDER_THICKNESS)
                    {
                        handled = true;
                        return (IntPtr)HTBOTTOMLEFT;
                    }
                    if (mousePos.X >= windowRect.Right - RESIZE_BORDER_THICKNESS && mousePos.X < windowRect.Right)
                    {
                        handled = true;
                        return (IntPtr)HTBOTTOMRIGHT;
                    }
                    handled = true;
                    return (IntPtr)HTBOTTOM;
                }
                if (mousePos.X >= windowRect.Left && mousePos.X < windowRect.Left + RESIZE_BORDER_THICKNESS)
                {
                    handled = true;
                    return (IntPtr)HTLEFT;
                }
                if (mousePos.X >= windowRect.Right - RESIZE_BORDER_THICKNESS && mousePos.X < windowRect.Right)
                {
                    handled = true;
                    return (IntPtr)HTRIGHT;
                }
            }

            return IntPtr.Zero;
        }

        private POINT GetMousePosition()
        {
            GetCursorPos(out POINT lpPoint);
            return lpPoint;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private void MainBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                // Adjust the Clip based on the new size
                border.Clip = new RectangleGeometry
                {
                    Rect = new Rect(0, 0, border.ActualWidth, border.ActualHeight),
                    RadiusX = border.CornerRadius.TopLeft,
                    RadiusY = border.CornerRadius.TopLeft
                };
            }
        }
    }
}