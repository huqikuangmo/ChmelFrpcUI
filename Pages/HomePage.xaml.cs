using FrpcUI.Class;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UIKitTutorials;

namespace FrpcUI.Pages
{
    public partial class HomePage : Page
    {
        // 定义API请求的URL
        private const string ApiUrl = "https://cf-v2.uapis.cn/userinfo";
        // 定义主色调
        private static readonly Color PrimaryColor = (Color)ColorConverter.ConvertFromString("#673AB7");
        // 播放图标几何形状
        private static readonly Geometry PlayIcon = Geometry.Parse("M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M10,16.5L16,12L10,7.5V16.5Z");
        // 停止图标几何形状
        private static readonly Geometry StopIcon = Geometry.Parse("M13,16V8H15V16H13M9,16V8H11V16H9M12,2A10,10 0 0,1 22,12A10,10 0 0,1 12,22A10,10 0 0,1 2,12A10,10 0 0,1 12,2M12,4A8,8 0 0,0 4,12A8,8 0 0,0 12,20A8,8 0 0,0 20,12A8,8 0 0,0 12,4Z");

        // 视图模型实例
        public LogingModelViewModel ViewModel { get; } = new LogingModelViewModel();
        // HTTP客户端实例
        private readonly HttpClient _httpClient = new HttpClient();
        // FRPC进程实例
        private Process _frpcProcess;
        // FRPC进程是否正在运行的标志
        private bool _isFrpcRunning;
        // 用于取消输出读取任务的令牌源
        private CancellationTokenSource _outputReadCancellationTokenSource;

        // 构造函数
        public HomePage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Application.Current.Exit += OnApplicationExit;

            // 启动时异步加载数据
            _ = LoadDataAsync();
        }

        // 页面加载事件处理
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RunFrpc_Click(EditButton, new RoutedEventArgs());
        }

        // 异步加载用户数据
        private async Task LoadDataAsync()
        {
            var savedLogin = ((App)Application.Current).LoadLoginState();
            if (savedLogin == null || string.IsNullOrEmpty(savedLogin.Token))
                return;

            var postData = new Dictionary<string, string> { { "token", savedLogin.Token } };
            var content = new FormUrlEncodedContent(postData);

            try
            {
                using (var response = await _httpClient.PostAsync(ApiUrl, content).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        ShowMessageBox($"发送失败，状态码：{response.StatusCode}");
                        return;
                    }

                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    ProcessApiResponse(responseBody);
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox($"请求发生错误: {ex.Message}");
            }
        }

        // 处理API响应
        private void ProcessApiResponse(string responseBody)
        {
            try
            {
                var json = JObject.Parse(responseBody);
                var state = json["state"]?.ToString().ToLower();

                switch (state)
                {
                    case "success":
                        HandleSuccessResponse(json);
                        break;
                    case "fail":
                        HandleFailResponse();
                        break;
                    default:
                        ShowMessageBox($"登录失败或未知状态: {state}");
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox($"解析响应失败: {ex.Message}");
            }
        }

        // 处理成功的API响应
        private void HandleSuccessResponse(JObject json)
        {
            if (json["data"] is not JObject data)
            {
                ShowMessageBox("返回的数据格式不正确，缺少'data'字段");
                return;
            }

            Dispatcher.Invoke(() =>
            {
                ViewModel.LoginModels.Add(new LoginModel
                {
                    Msg = json["msg"]?.ToString(),
                    Code = json["code"]?.ToObject<int>() ?? 0,
                    RealName = data["realname"]?.ToString(),
                    tunnelCount = data["tunnelCount"]?.ToString() ?? string.Empty,
                    Name = data["username"]?.ToString() ?? string.Empty,
                    Token = data["usertoken"]?.ToString() ?? string.Empty,
                    QianDao = bool.TryParse(data["qiandao"]?.ToString(), out bool qianDaoResult) ? qianDaoResult : false,
                    UserGroup = data["usergroup"]?.ToString() ?? string.Empty,
                    Integral = data["integral"]?.ToObject<int>() ?? 0,
                    AbroadBandwidth = data["abroadbandwidth"]?.ToObject<int>() ?? 0,
                    Bandwidth = data["bandwidth"]?.ToObject<int>() ?? 0,
                    QQ = data["qq"]?.ToObject<long>() ?? 0,
                    Tunnel = data["tunnel"]?.ToObject<int>() ?? 0,
                    UsedTunnel = data["usedtunnel"]?.ToObject<int>() ?? 0,
                    Mail = data["email"]?.ToString() ?? string.Empty,
                    UserID = data["userid"]?.ToObject<int>() ?? 0,
                    UserImg = data["userimg"]?.ToString() ?? string.Empty,
                    IdentityID = data["identityID"]?.ToString() ?? string.Empty,
                    DateOut = data["regtime"] == null ? (DateTime?)null : DateTime.Parse(data["regtime"].ToString())
                });
            });
        }

        // 处理失败的API响应
        private void HandleFailResponse()
        {
            Dispatcher.Invoke(() =>
            {
                var currentWindow = Window.GetWindow(this);
                if (currentWindow != null)
                {
                    new MainWindow().Show();
                    currentWindow.Close();
                }
            });
        }

        // 显示或隐藏Token点击事件处理
        public void ShowToken_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Parent is Panel parent)
            {
                if (parent.FindName("token") is TextBlock token)
                {
                    token.Visibility = token.Visibility == Visibility.Visible
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                }
            }
        }

        // 复制Token到剪贴板鼠标左键按下事件处理
        private void Token_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock { Text: { } text })
            {
                Clipboard.SetText(text);
            }
        }

        // 运行或停止FRPC点击事件处理
        public async void RunFrpc_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            if (_isFrpcRunning)
            {
                await StopFrpcProcessAsync();
                UpdateButtonContent(button, "运行", PlayIcon, PrimaryColor);
            }
            else
            {
                if (StartFrpcProcess())
                {
                    _isFrpcRunning = true;
                    UpdateButtonContent(button, "停止", StopIcon, Colors.Red);
                    _ = ReadOutputAsync(_frpcProcess.StandardOutput.BaseStream);
                }
            }
        }

        // 启动FRPC进程
        private bool StartFrpcProcess()
        {
            try
            {
                var frpcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Frpc", "frpc.exe");
                var workingDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Frpc");

                _frpcProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = frpcPath,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = workingDir
                    },
                    EnableRaisingEvents = true
                };

                _frpcProcess.Exited += (s, e) => Dispatcher.Invoke(() => _isFrpcRunning = false);
                _outputReadCancellationTokenSource = new CancellationTokenSource();

                return _frpcProcess.Start();
            }
            catch (Exception ex)
            {
                ShowMessageBox($"启动FRPC失败: {ex.Message}");
                return false;
            }
        }

        // 异步停止FRPC进程
        private async Task StopFrpcProcessAsync()
        {
            if (_frpcProcess == null) return;

            try
            {
                _outputReadCancellationTokenSource?.Cancel();

                if (!_frpcProcess.HasExited)
                {
                    _frpcProcess.Kill();
                    await _frpcProcess.WaitForExitAsync().ConfigureAwait(false);
                }

                cmdOutput.Clear();
            }
            catch (Exception ex)
            {
                ShowMessageBox($"停止FRPC失败: {ex.Message}");
            }
            finally
            {
                _isFrpcRunning = false;
                _frpcProcess?.Dispose();
                _frpcProcess = null;
            }
        }

        // 异步读取FRPC进程输出
        private async Task ReadOutputAsync(Stream stream)
        {
            try
            {
                using var reader = new StreamReader(stream);
                var buffer = new char[4096];

                while (!_outputReadCancellationTokenSource.IsCancellationRequested)
                {
                    var bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    var text = new string(buffer, 0, bytesRead);
                    AppendToTextBox(text);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => ShowMessageBox($"读取输出失败: {ex.Message}"));
            }
        }

        // 更新按钮内容
        private void UpdateButtonContent(Button button, string text, Geometry icon, Color color)
        {
            Dispatcher.Invoke(() =>
            {
                button.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                    {
                        new System.Windows.Shapes.Path
                        {
                            Data = icon,
                            Fill = new SolidColorBrush(color),
                            Height = 25,
                            Width = 25,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 0, 5, 0)
                        },
                        new TextBlock
                        {
                            Text = text,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 16,
                            FontWeight = FontWeights.Bold,
                            Foreground = new SolidColorBrush(color)
                        }
                    }
                };
            });
        }

        // 将文本追加到命令输出文本框
        private void AppendToTextBox(string text)
        {
            Dispatcher.Invoke(() =>
            {
                cmdOutput.AppendText(text);
                cmdOutput.ScrollToEnd();
            });
        }

        // 显示消息框
        private void ShowMessageBox(string message)
        {
            Dispatcher.Invoke(() => MessageBox.Show(message));
        }

        // 应用程序退出事件处理
        private async void OnApplicationExit(object sender, ExitEventArgs e)
        {
            if (_isFrpcRunning && _frpcProcess != null)
            {
                await StopFrpcProcessAsync();
            }
            _httpClient.Dispose();
        }
    }
}
