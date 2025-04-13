using FrpcUI.Class;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;


namespace UIKitTutorials
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public class UserProfile
        {
            public string Name { get; set; }
            public string Mail { get; set; }
            public string UserImg { get; set; }
        }

        public ObservableCollection<UserProfile> UserProfiles { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // 在构造函数中设置初始页面
            PagesNavigation.Source = new Uri("Pages/HomePage.xaml", UriKind.Relative);
            // 隐藏导航栏但保留内容显示
            PagesNavigation.NavigationUIVisibility = NavigationUIVisibility.Hidden;

            // 加载登录状态
            LoginModel savedLogin = ((App)Application.Current).LoadLoginState();

            // 初始化数据源
            UserProfiles = new ObservableCollection<UserProfile>
            {
                 new UserProfile { Name = savedLogin.RealName, Mail = savedLogin.Mail, UserImg = savedLogin.UserImg }
            };

            // 设置数据上下文
            this.DataContext = this;
        }






        private void btnPageClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void btnPageRestore_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
                Window.GetWindow(this).WindowState = Window.GetWindow(this).WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        }

        private void btnPageMinimize_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Window.GetWindow(this)?.DragMove();
            }
        }

        private void PagesNavigation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 设置Handled属性为true，阻止事件冒泡
            e.Handled = true;
        }

        private void rdHome_Click(object sender, RoutedEventArgs e)
        {

            PagesNavigation.Source = new Uri("Pages/HomePage.xaml", UriKind.Relative);
        }


        private void SuiDao_Click(object sender, RoutedEventArgs e)
        {
            if (rdSub1.IsChecked == true || rdSub2.IsChecked == true)
            {

            }
            else
            {
                // 设置子按钮组可见状态并展开高度动画
                SubButtonsPanel.Visibility = Visibility.Visible;

                var expandAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 100, // 根据UI高度调整
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                SubButtonsPanel.BeginAnimation(HeightProperty, expandAnimation);
            }

        }

        private void UnSuiDao_Click(object sender, RoutedEventArgs e)
        {
            if (rdSub1.IsChecked == true || rdSub2.IsChecked == true)
            {

            }
            else
            {
                var collapseAnimation = new DoubleAnimation
                {
                    From = SubButtonsPanel.ActualHeight,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                collapseAnimation.Completed += (s, _) =>
                {
                    SubButtonsPanel.Visibility = Visibility.Collapsed;
                };

                SubButtonsPanel.BeginAnimation(HeightProperty, collapseAnimation);
            }
        }

        private void SuiDaoLieBiao_Click(object sender, RoutedEventArgs e)
        {

            PagesNavigation.Source = new Uri("Pages/SuiDaoPage.xaml", UriKind.Relative);
        }

        private void Peizhiwenjian_Click(object sender, RoutedEventArgs e)
        {

            PagesNavigation.Source = new Uri("Pages/Peizhiwenjian.xaml", UriKind.Relative);
        }

        private void YuMing_Click(object sender, RoutedEventArgs e)
        {

        }



    }
}
