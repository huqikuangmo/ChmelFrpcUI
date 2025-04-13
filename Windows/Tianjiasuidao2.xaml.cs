using FrpcUI.Class;
using System;
using System.Windows;

namespace FrpcUI.Windows
{
    /// <summary>
    /// Tianjiasuidao2.xaml 的交互逻辑
    /// </summary>
    public partial class Tianjiasuidao2 : Window
    {
        public Data Data = new Data();

        public string Name { get; } // 添加公共属性来存储 name

        public Tianjiasuidao2(String name)
        {
            Name = name;

            InitializeComponent();
            //LoadDataAsync();
            this.DataContext = new NodeViewModel(Name);

        }

        public void Close_btn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Previous_btn(object sender, RoutedEventArgs e)
        {
            // 实例化Window
            Tianjiasuidao tianjiasuiddao = new Tianjiasuidao();
            tianjiasuiddao.Show();
            this.Close();
        }

        public void Next_btn(object sender, RoutedEventArgs e)
        {
            string Jianzhan = jianzhan.Text;
            // 实例化Window
            Cuangjiansuidao cuangjiansuidao = new Cuangjiansuidao(Name, Jianzhan);
            cuangjiansuidao.Show();
            this.Close();
        }

        //private async void LoadDataAsync()
        //{
        //    // 加载登录状态
        //    LoginModel savedLogin = ((App)Application.Current).LoadLoginState();
        //    string token = savedLogin.Token;
        //    string urlAPI = "https://cf-v2.uapis.cn/nodeinfo";
        //    var postData = new Dictionary<string, string>
        //    {
        //        { "token", token },
        //        { "node", Name }
        //    };
        //    var content = new FormUrlEncodedContent(postData);

        //    try
        //    {
        //        using (HttpResponseMessage response = await httpClient.PostAsync(urlAPI, content))
        //        {
        //            if (response.IsSuccessStatusCode)
        //            {
        //                string responseBody = await response.Content.ReadAsStringAsync();
        //                JObject keyValuePairs = JObject.Parse(responseBody);
        //                string state = keyValuePairs["state"].ToString();

        //                switch (state.ToLower())
        //                {
        //                    case "success":
        //                        if (keyValuePairs["data"] != null)
        //                        {
        //                            JObject dataArray = keyValuePairs["data"] as JObject;
        //                            // 检查data是否为null
        //                            if (dataArray != null)
        //                            {


        //                                    JObject data = dataArray as JObject;
        //                                    if (data != null)
        //                                    {
        //                                        Data data1 = new Data
        //                                        {
        //                                            Id = int.TryParse(data["id"]?.ToString(), out int id) ? id : 0,
        //                                            Area = data["area"]?.ToString() ?? string.Empty,
        //                                            China = data["china"]?.ToString() ?? string.Empty,
        //                                            Fangyu = data["fangyu"]?.ToString() ?? string.Empty,
        //                                            Notes = data["notes"]?.ToString() ?? string.Empty,
        //                                            Ip = data["ip"]?.ToString() ?? string.Empty,
        //                                            Ipv6 = data["ipv6"]?.ToString() ?? string.Empty,
        //                                            Name = data["name"]?.ToString() ?? string.Empty,
        //                                            Nodegroup = data["nodegroup"]?.ToString() ?? string.Empty,
        //                                            State = data["state"]?.ToString() ?? string.Empty,
        //                                            Toowhite = bool.TryParse(data["toowhite"]?.ToString(), out bool toowhite) ? toowhite : false,
        //                                            Udp = data["udp"]?.ToString() ?? string.Empty,
        //                                            Rport = data["rport"]?.ToString() ?? string.Empty,
        //                                            Web = data["web"]?.ToString() ?? string.Empty,
        //                                            RealIp = data["realip"]?.ToString() ?? string.Empty,

        //                                        };
        //                                        Data.Datas.Add(data1);

        //                                }
        //                            }
        //                            else
        //                            {
        //                                MessageBox.Show("返回的数据格式不正确，缺少'data'字段");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            MessageBox.Show("返回的数据格式不正确，缺少'data'字段");
        //                        }
        //                        break;
        //                    case "fail1":
        //                        MessageBox.Show("获取隧道失败");
        //                        break;
        //                    case "error":
        //                        MessageBox.Show("获取隧道失败");
        //                        break;
        //                    case "fail":
        //                        MessageBox.Show("获取隧道失败");
        //                        break;
        //                    default:
        //                        MessageBox.Show("未知的状态码：" + state);
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show("发送失败，状态码：" + response.StatusCode);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("请求发生错误: " + ex.Message);
        //    }
        //}
    }
}
