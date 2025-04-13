using FrpcUI.Class;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace UIKitTutorials
{
    /// <summary>
    /// 应用程序主入口
    /// </summary>
    public partial class App : Application
    {
        private const string LoginStateFile = "loginState.json";
        private const int WindowWidth = 900;
        private const int WindowHeight = 550;

        public static Window MainWindowInstance { get; set; }

        /// <summary>
        /// 检查并删除过期的登录状态文件（超过5天未修改）
        /// </summary>
        /// <returns>是否执行了删除操作</returns>
        public bool CheckAndDeleteExpiredLoginState()
        {
            try
            {
                using var isoFile = IsolatedStorageFile.GetUserStoreForApplication();

                if (!isoFile.FileExists(LoginStateFile))
                    return false;

                // 获取文件最后修改时间
                DateTimeOffset lastWriteTimeOffset = isoFile.GetLastWriteTime(LoginStateFile);
                DateTime lastWriteTime = lastWriteTimeOffset.LocalDateTime;
                TimeSpan timeSinceLastWrite = DateTime.Now - lastWriteTime;

                // 如果超过5天则删除
                if (timeSinceLastWrite.TotalDays > 5)
                {
                    isoFile.DeleteFile(LoginStateFile);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查删除过期登录状态失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 从独立存储加载登录状态
        /// </summary>
        public LoginModel LoadLoginState()
        {
            try
            {
                using var isoFile = IsolatedStorageFile.GetUserStoreForApplication();

                if (!isoFile.FileExists(LoginStateFile))
                    return null;

                using var stream = isoFile.OpenFile(LoginStateFile, FileMode.Open);
                using var reader = new StreamReader(stream);

                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<LoginModel>(json);
            }
            catch (Exception ex)
            {
                // 在实际应用中应该记录日志
                Console.WriteLine($"加载登录状态失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 删除保存的登录状态文件
        /// </summary>
        /// <returns>是否成功删除</returns>
        public bool DeleteLoginState()
        {
            try
            {
                using var isoFile = IsolatedStorageFile.GetUserStoreForApplication();

                if (!isoFile.FileExists(LoginStateFile))
                    return false;

                isoFile.DeleteFile(LoginStateFile);
                return true;
            }
            catch (Exception ex)
            {
                // 在实际应用中应该记录日志
                Console.WriteLine($"删除登录状态失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 创建登录窗口
        /// </summary>
        private NavigationWindow CreateLoginWindow()
        {
            return new NavigationWindow
            {
                Source = new Uri("Pages/LoginPage.xaml", UriKind.Relative),
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                Width = WindowWidth,
                Height = WindowHeight,
                Style = (Style)FindResource("HiddenNavigationStyle")
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var savedLogin = LoadLoginState();

                if (savedLogin?.Msg == "登录成功")
                {
                    MainWindowInstance = new MainWindow();
                }
                else
                {
                    MainWindowInstance = CreateLoginWindow();
                }

                MainWindowInstance.Show();
            }
            catch (Exception ex)
            {
                // 在实际应用中应该显示友好的错误信息并记录日志
                MessageBox.Show($"应用程序启动失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}