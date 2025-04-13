using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace FrpcUI.Class
{
    /// <summary>
    /// 处理隧道相关网络请求的服务类
    /// </summary>
    public class SuidaoService
    {
        private readonly HttpClient _httpClient;

        public SuidaoService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 加载隧道数据
        /// </summary>
        public async Task<JObject> LoadTunnelDataAsync(string token)
        {
            string urlAPI = "https://cf-v2.uapis.cn/tunnel";
            var postData = new Dictionary<string, string> { { "token", token } };
            var content = new FormUrlEncodedContent(postData);

            try
            {
                using (var response = await _httpClient.PostAsync(urlAPI, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return JObject.Parse(responseBody);
                    }
                    MessageBox.Show($"发送失败，状态码：{response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"请求发生错误: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 删除隧道
        /// </summary>
        public async Task<bool> DeleteTunnelAsync(int id, int userId, string token)
        {
            string urlAPI = "https://cf-v1.uapis.cn/api/deletetl.php";
            var postData = new Dictionary<string, string>
            {
                { "token", token },
                { "nodeid", id.ToString() },
                { "userid", userId.ToString() }
            };
            var content = new FormUrlEncodedContent(postData);

            try
            {
                using (var response = await _httpClient.PostAsync(urlAPI, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var keyValuePairs = JObject.Parse(responseBody);
                        string state = keyValuePairs["code"].ToString();

                        switch (state.ToLower())
                        {
                            case "200":
                                MessageBox.Show("删除隧道成功");
                                return true;
                            case "400":
                            case "error":
                            case "fail":
                                MessageBox.Show("删除隧道失败");
                                return false;
                            default:
                                MessageBox.Show($"未知的状态码：{state}");
                                return false;
                        }
                    }
                    MessageBox.Show($"发送失败，状态码：{response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"请求发生错误: {ex.Message}");
                return false;
            }
        }
    }
}