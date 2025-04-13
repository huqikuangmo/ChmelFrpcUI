using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using UIKitTutorials;

namespace FrpcUI.Class
{
    public class PeizhiModel
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BaseApiUrl = "https://cf-v2.uapis.cn";

        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        public string Node { get; }
        public string Name { get; }

        public ObservableCollection<PeizhiModel> PeizhiList { get; } = new ObservableCollection<PeizhiModel>();

        // 主构造函数
        public PeizhiModel(string node, string name = null)
        {
            Node = node;
            Name = name;
        }

        // 私有构造函数用于JSON反序列化
        private PeizhiModel()
        {
        }

        public async Task<string> LoadDataAsync()
        {
            try
            {
                var savedLogin = ((App)Application.Current).LoadLoginState();
                var postData = CreatePostData(savedLogin.Token);

                var response = await _httpClient.PostAsync($"{BaseApiUrl}/tunnel_config",
                    new FormUrlEncodedContent(postData));

                return await ProcessResponse(response);
            }
            catch (HttpRequestException ex)
            {
                ShowErrorMessage($"网络请求失败: {ex.Message}");
            }
            catch (JsonException ex)
            {
                ShowErrorMessage($"数据解析失败: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"发生未知错误: {ex.Message}");
            }
            return string.Empty;
        }

        private Dictionary<string, string> CreatePostData(string token)
        {
            var postData = new Dictionary<string, string>
            {
                { "token", token },
                { "node", Node }
            };

            if (!string.IsNullOrEmpty(Name))
            {
                postData.Add("tunnel_names", Name);
            }

            return postData;
        }

        private async Task<string> ProcessResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                ShowErrorMessage($"请求失败，状态码：{response.StatusCode}");
                return string.Empty;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(responseBody);

            if (!ValidateResponseState(responseJson))
                return string.Empty;

            // 更新当前实例属性
            this.Code = responseJson["code"]?.ToObject<long>() ?? 0;
            this.Data = responseJson["data"]?.ToString() ?? string.Empty;
            this.Msg = responseJson["msg"]?.ToString() ?? string.Empty;
            this.State = responseJson["state"]?.ToString() ?? string.Empty;

            return this.Data;
        }

        private bool ValidateResponseState(JObject responseJson)
        {
            var state = responseJson["state"]?.ToString().ToLower();

            if (state == "success")
                return true;

            var errorMessage = state switch
            {
                "fail1" => "认证失败",
                "error" => "服务器错误",
                "fail" => "获取隧道失败",
                _ => $"未知的状态码：{state}"
            };

            ShowErrorMessage(errorMessage);
            return false;
        }

        //private void UpdatePeizhiList(JObject responseJson)
        //{
        //    PeizhiList.Clear();

        //    // 使用当前实例的Node和Name创建新实例
        //    var peizhi = new PeizhiModel(this.Node, this.Name)
        //    {
        //        Code = responseJson["code"]?.ToObject<long>() ?? 0,
        //        Data = responseJson["data"]?.ToString() ?? string.Empty,
        //        Msg = responseJson["msg"]?.ToString() ?? string.Empty,
        //        State = responseJson["state"]?.ToString() ?? string.Empty
        //    };

        //    PeizhiList.Add(peizhi);
        //}

        private static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}