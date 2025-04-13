using FrpcUI.Class;
using FrpcUI.Windows;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;

namespace FrpcUI
{
    /// <summary>
    /// Tianjiasuidao.xaml 的交互逻辑
    /// </summary>
    public partial class Tianjiasuidao : Window
    {
        public Datum Datum = new Datum();

        private readonly HttpClient httpClient;
        public Tianjiasuidao()
        {
            this.Activate();
            httpClient = new HttpClient();
            InitializeComponent();
            this.DataContext = Datum;
            LoadDataAsync();

        }

        private async void LoadDataAsync()
        {

            string urlAPI = "https://cf-v2.uapis.cn/node";


            try
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(urlAPI))
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
                                    JArray dataArray = keyValuePairs["data"] as JArray;
                                    // 检查data是否为null
                                    if (dataArray != null)
                                    {
                                        foreach (JToken item in dataArray)
                                        {
                                            JObject data = item as JObject;
                                            if (data != null)
                                            {
                                                Datum tianjiasuidao = new Datum
                                                {
                                                    Id = data["id"]?.ToObject<int>() ?? 0,
                                                    Area = data["name"]?.ToString() ?? string.Empty,
                                                    China = data["china"]?.ToString() ?? string.Empty,
                                                    Name = data["name"]?.ToString() ?? string.Empty,

                                                    Nodegroup = data["nodegroup"]?.ToString() ?? string.Empty,
                                                    Udp = data["udp"]?.ToString() ?? string.Empty,
                                                    Web = data["web"]?.ToString() ?? string.Empty,

                                                };
                                                Datum.Datums.Add(tianjiasuidao);
                                            }
                                        }
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
                                break;
                            case "fail1":
                                MessageBox.Show("获取隧道失败");
                                break;
                            case "error":
                                MessageBox.Show("获取隧道失败");
                                break;
                            case "fail":
                                MessageBox.Show("获取隧道失败");
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

        private void Suidao_Click(object sender, RoutedEventArgs e)
        {
            // 获取点击的按钮
            Button button = sender as Button;

            // 获取按钮所在的 StackPanel
            StackPanel stackPanel = button.Parent as StackPanel;

            // 获取 StackPanel 的 DataContext，即节点的数据对象
            var datum = stackPanel.DataContext as Datum;

            // 如果获取到了数据对象，则获取 Name 属性的值
            if (datum != null)
            {
                // 实例化你的UserControl
                Tianjiasuidao2 Tianjiasuidao = new Tianjiasuidao2(datum.Name);

                Tianjiasuidao.Show();
                this.Close();

            }
            else
            {
                MessageBox.Show("未找到节点数据对象。");
            }
        }



    }
}
