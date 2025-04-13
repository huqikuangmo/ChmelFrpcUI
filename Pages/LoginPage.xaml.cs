using FrpcUI.Class;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace UIKitTutorials.Pages
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Page
    {
        public LogingModelViewModel viewModel = new LogingModelViewModel();
        private readonly HttpClient httpClient;

        public LoginPage()
        {
            InitializeComponent();
            httpClient = new HttpClient();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Window currentWindow = Window.GetWindow(this);
                currentWindow.DragMove();
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Visibility = Visibility.Collapsed; // 隐藏TextBlock文本
            txtPassword.Focus(); // 将焦点设置到PasswordBox上
        }

        private async void SaveLoginState(LoginModel login)
        {
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();

            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("loginState.json", FileMode.Create, isoFile))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string json = JsonConvert.SerializeObject(login);
                await writer.WriteAsync(json);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string userName = textUser.Text;
            string passWord = textPassword.Password;
            string urlAPI = "https://cf-v2.uapis.cn/login";

            var postData = new Dictionary<string, string>
            {
                { "username", userName },
                { "password", passWord }
            };

            var content = new FormUrlEncodedContent(postData);

            try
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(urlAPI, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject keyValuePairs = JObject.Parse(responseBody);
                        string state = keyValuePairs["state"].ToString();

                        switch (state.ToLower())
                        {
                            case "success":
                                if (keyValuePairs["data"] != null)
                                {
                                    JObject data = keyValuePairs["data"] as JObject;
                                    // 检查data是否为null
                                    if (data != null)
                                    {
                                        LoginModel login = new LoginModel
                                        {
                                            Msg = keyValuePairs["msg"].ToString(),
                                            Code = keyValuePairs["code"].ToObject<int>(),
                                            RealName = data["username"]?.ToString() ?? string.Empty,
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
                                        };
                                        viewModel.LoginModels.Add(login);
                                        SaveLoginState(login); // 存储登录状态
                                    }
                                    else
                                    {
                                        MessageBox.Show("返回的数据格式不正确，缺少'data'字段");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("返回的数据格式不正确，缺少'data'字段");
                                }
                                Window currentWindow = Window.GetWindow(this);
                                if (currentWindow != null)
                                {
                                    new MainWindow().Show();
                                    currentWindow.Close();
                                }
                                break;

                            case "fail1":
                            case "error":
                            case "fail":
                                MessageBox.Show("登录失败");
                                break;
                            default:
                                MessageBox.Show("未知的状态码：" + state);
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("发送失败，状态码：" + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("请求发生错误: " + ex.Message);
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window currentWindow = Window.GetWindow(this);
            currentWindow.Close();
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUser.Visibility = string.IsNullOrEmpty(txtUser.Text) ? Visibility.Visible : Visibility.Collapsed;
            textUser.Focus();
        }

        private void Button_Click_Up(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://preview.panel.chmlfrp.cn/sign",
                UseShellExecute = true
            });
        }
    }
}

