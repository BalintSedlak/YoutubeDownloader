using System.Windows;

namespace YoutubeDownloader.Wpf.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            _MainGrid.DataContext = mainWindowViewModel;
        }

        private void _MainGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.DragMove();
        }

        private void _MinimalizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void _CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ScrollViewer_ScrollChanged(object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0)
            {
                _ConsoleOutputScrollViewer.ScrollToVerticalOffset(_ConsoleOutputScrollViewer.ExtentHeight);
            }
        }
    }
}
