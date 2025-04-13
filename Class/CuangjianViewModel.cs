using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UIKitTutorials;

namespace FrpcUI.Class
{
    public class CuangjianViewModel
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private bool _creationStatus; // 新增字段存储创建状态
        public string TunnelName { get; }
        public string InnerPort { get; }
        public string OuterPort { get; }
        public string LocalIP { get; }
        public string PortType { get; }
        public string Node { get; }
        public string YuMing { get; }
        public bool Shujujiami { get; }
        public bool Shujuyasuo { get; }
        public string ExtraParams { get; }

        public CuangjianViewModel(string tunnelName, string innerPort, string localIP, string portType, string outerPort, string selectedNode, string yuMing, bool shujujiami, bool shujuyasuo, string extraParams)
        {
            TunnelName = tunnelName;
            InnerPort = innerPort;
            OuterPort = outerPort;
            LocalIP = localIP;
            PortType = portType;
            Node = selectedNode;
            YuMing = yuMing;
            Shujujiami = shujujiami;
            Shujuyasuo = shujuyasuo;
            ExtraParams = extraParams;
            _creationStatus = LoadNodeDataAsync().GetAwaiter().GetResult(); // 构造函数中执行一次并将结果存储
        }

        private async Task<bool> LoadNodeDataAsync()
        {
            try
            {
                var savedLogin = ((App)Application.Current).LoadLoginState();
                if (string.IsNullOrWhiteSpace(savedLogin?.Token))
                {
                    MessageBox.Show("无效的登录状态");
                    return false;
                }

                const string urlAPI = "https://cf-v2.uapis.cn/create_tunnel";
                var postData = new JObject();
                if (PortType == "TCP" || PortType == "UDP")
                {
                    postData = new JObject
                                {
                                    { "token", savedLogin.Token },
                                    { "tunnelname", TunnelName },
                                    { "node", Node },
                                    { "localip", LocalIP },
                                    { "porttype", PortType },
                                    { "localport", InnerPort },
                                    { "remoteport", OuterPort },

                                    { "encryption", Shujujiami },
                                    { "compression", Shujuyasuo },
                                    { "extraparams", ExtraParams }
                                };
                }
                else
                {
                    postData = new JObject
                                {
                                    { "token", savedLogin.Token },
                                    { "tunnelname", TunnelName },
                                    { "node", Node },
                                    { "localip", LocalIP },
                                    { "porttype", PortType },
                                    { "localport", InnerPort },

                                    { "banddomain", YuMing },
                                    { "encryption", Shujujiami },
                                    { "compression", Shujuyasuo },
                                    { "extraparams", ExtraParams }
                                };
                }


                using var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");
                // 将请求体转换为字符串，以便查看
                string requestBody = await content.ReadAsStringAsync();
                //Debug.WriteLine($"Request URL: {urlAPI}");
                Debug.WriteLine($"Request Body: {requestBody}");
                using var response = await _httpClient.PostAsync(urlAPI, content).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject keyValuePairs = JObject.Parse(responseBody);
                string state = keyValuePairs["state"].ToString();


                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"发送失败，状态码：{response.StatusCode}");
                    return false;
                }
                switch (state.ToLower().Trim()) // 使用Trim()去除可能的前后空格
                {
                    case "success":
                        MessageBox.Show($"创建成功，状态码：{response.StatusCode}");
                        return true;
                    case "fail":
                        MessageBox.Show($"创建失败，请检查输入是否正确{response.StatusCode}");
                        return false;
                    default: // 增加默认情况
                        MessageBox.Show($"创建失败，请检查输入是否正确{response.StatusCode}");
                        return false;
                }

            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"网络请求错误: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}");
                return false;
            }
        }

        public string Status()
        {
            return _creationStatus.ToString(); // 直接返回之前存储的结果
        }
    }
}

