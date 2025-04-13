using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using UIKitTutorials;

public class NodeViewModel
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public string Name { get; }
    public ObservableCollection<NodeProperty> NodeProperties { get; } = new ObservableCollection<NodeProperty>();

    public NodeViewModel(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _ = LoadNodeDataAsync(); // 使用丢弃操作符表示我们不等待这个任务
    }

    private async Task LoadNodeDataAsync() // 改为返回Task以便调用者可以await
    {
        try
        {
            var savedLogin = ((App)Application.Current).LoadLoginState();
            if (string.IsNullOrWhiteSpace(savedLogin?.Token))
            {
                MessageBox.Show("无效的登录状态");
                return;
            }

            const string urlAPI = "https://cf-v2.uapis.cn/nodeinfo";
            var postData = new Dictionary<string, string>
            {
                { "token", savedLogin.Token },
                { "node", Name }
            };

            using var content = new FormUrlEncodedContent(postData);
            using var response = await _httpClient.PostAsync(urlAPI, content).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show($"发送失败，状态码：{response.StatusCode}");
                return;
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            ProcessResponse(responseBody);
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show($"网络请求错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"发生错误: {ex.Message}");
        }
    }

    private void ProcessResponse(string responseBody)
    {
        try
        {
            var json = JObject.Parse(responseBody);
            var state = json["state"]?.ToString().ToLowerInvariant();

            switch (state)
            {
                case "success":
                    if (json["data"] is JObject data)
                    {
                        Application.Current.Dispatcher.Invoke(() => UpdateNodeProperties(data));
                    }
                    else
                    {
                        MessageBox.Show("返回的数据格式不正确，缺少'data'字段");
                    }
                    break;
                case "fail1":
                case "error":
                case "fail":
                    MessageBox.Show("获取隧道失败");
                    break;
                default:
                    MessageBox.Show($"未知的状态码：{state}");
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"解析响应时出错: {ex.Message}");
        }
    }

    private void UpdateNodeProperties(JObject data)
    {
        NodeProperties.Clear();

        // 使用预分配的属性列表减少多次集合操作
        var properties = new List<NodeProperty>(13) // 预先知道要添加13个属性
        {
            CreateProperty("节点名", data["name"]?.ToString(), 0, 0),
            CreateProperty("地区", data["area"]?.ToString(), 0, 2),
            CreateProperty("权限组", data["nodegroup"]?.ToString(), 0, 4),

            CreateProperty("防御", GetYesNoDisplay(data["fangyu"], "有防御", "无防御"), 1, 0),
            CreateProperty("建站", GetYesNoDisplay(data["web"], "可以建站", "不可建站"), 1, 2),
            CreateProperty("UDP", data["udp"]?.ToString(), 1, 4),

            CreateProperty("域名过白", GetBoolDisplay(data["toowhite"], "需要备案过白", "域名无需备案过白"), 2, 0),
            CreateProperty("端口限制", data["rport"]?.ToString(), 2, 2),
            CreateProperty("域名", data["ip"]?.ToString(), 2, 4),

            CreateProperty("IP", data["realIp"]?.ToString(), 3, 0),
            CreateProperty("IPv6", GetIpv6Display(data["ipv6"]), 3, 2),
            CreateProperty("带宽", GetYesNoDisplay(data["china"], "国内带宽", "国外带宽"), 3, 4),

            CreateProperty("介绍", data["notes"]?.ToString(), 4, 0)
        };

        foreach (var prop in properties)
        {
            if (prop != null)
            {
                NodeProperties.Add(prop);
            }
        }
    }

    // 辅助方法减少重复代码
    private static NodeProperty CreateProperty(string label, string value, int row, int column)
    {
        return string.IsNullOrEmpty(value) ? null : new NodeProperty
        {
            Label = label,
            Value = value,
            RowIndex = row,
            ColumnIndex = column
        };
    }

    private static string GetYesNoDisplay(JToken token, string yesText, string noText)
    {
        return (token?.ToString() ?? "no").Equals("yes", StringComparison.OrdinalIgnoreCase) ? yesText : noText;
    }

    private static string GetBoolDisplay(JToken token, string trueText, string falseText)
    {
        return bool.TryParse(token?.ToString(), out var result) && result ? trueText : falseText;
    }

    private static string GetIpv6Display(JToken token)
    {
        var ipv6 = token?.ToString();
        return string.IsNullOrWhiteSpace(ipv6) ? "此节点没有IPV6" : ipv6;
    }
}

public class NodeProperty
{
    public string Label { get; set; }
    public string Value { get; set; }
    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
}