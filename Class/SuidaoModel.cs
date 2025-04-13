using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using UIKitTutorials;

namespace FrpcUI.Class
{
    // SuidaoModel 类表示一个隧道的模型，包含隧道的各种属性信息
    public class SuidaoModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LocalIP { get; set; }
        public string Type { get; set; }
        public int NPort { get; set; }
        public string DORP { get; set; }
        public string Node { get; set; }
        public string State { get; set; }
        public int UserID { get; set; }
        public bool UseEncryption { get; set; }
        public bool UseCompression { get; set; }
        public string AP { get; set; }
        public DateTime? Uptime { get; set; }
        public string ClientVersion { get; set; }
        public long TodayTrafficIn { get; set; }
        public long TodayTrafficOut { get; set; }
        public int CurConns { get; set; }
        public string NodeState { get; set; }
        public string ConnectUrl { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
    }

    // SuidaoViewModel 类用于管理隧道数据，并提供数据绑定支持
    public partial class SuidaoViewModel : INotifyPropertyChanged
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BaseApiUrl = "https://cf-v2.uapis.cn";

        private SuidaoModel _selectedNode;
        private SuidaoModel _selectedSuidao;
        private ObservableCollection<SuidaoModel> _uniqueNodeModels;
        private ObservableCollection<SuidaoModel> _filteredSuidaoModels;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<SuidaoModel> SuidaoModels { get; } = new ObservableCollection<SuidaoModel>();
        public ObservableCollection<SuidaoModel> UniqueNodeModels
        {
            get => _uniqueNodeModels;
            private set
            {
                if (_uniqueNodeModels != value)
                {
                    _uniqueNodeModels = value;
                    OnPropertyChanged(nameof(UniqueNodeModels));
                }
            }
        }

        public ObservableCollection<SuidaoModel> FilteredSuidaoModels
        {
            get => _filteredSuidaoModels;
            private set
            {
                if (_filteredSuidaoModels != value)
                {
                    _filteredSuidaoModels = value;
                    OnPropertyChanged(nameof(FilteredSuidaoModels));
                }
            }
        }



        public SuidaoModel SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (_selectedNode != value)
                {
                    _selectedNode = value;
                    OnPropertyChanged(nameof(SelectedNode));
                    UpdateFilteredSuidaoModels(value?.Node);
                    SelectedSuidao = FilteredSuidaoModels.FirstOrDefault();
                }
            }
        }

        public SuidaoModel SelectedSuidao
        {
            get => _selectedSuidao;
            set
            {
                if (_selectedSuidao != value)
                {
                    _selectedSuidao = value;
                    OnPropertyChanged(nameof(SelectedSuidao));
                }
            }
        }

        public SuidaoViewModel()
        {
            UniqueNodeModels = new ObservableCollection<SuidaoModel>();
            FilteredSuidaoModels = new ObservableCollection<SuidaoModel>();
        }

        // 根据选定的节点更新过滤后的隧道模型列表
        private void UpdateFilteredSuidaoModels(string nodeName)
        {
            var filtered = string.IsNullOrEmpty(nodeName)
                ? SuidaoModels.ToList()
                : SuidaoModels.Where(x => x.Node == nodeName).ToList();

            FilteredSuidaoModels = new ObservableCollection<SuidaoModel>(filtered);
        }

        // 从所有隧道模型中提取唯一的节点模型并更新列表
        public void UpdateUniqueNodes()
        {
            var uniqueNodes = SuidaoModels
                .GroupBy(x => x.Node)
                .Select(g => g.First())
                .ToList();

            UniqueNodeModels = new ObservableCollection<SuidaoModel>(uniqueNodes);

            if (UniqueNodeModels.Count > 0)
            {
                SelectedNode = UniqueNodeModels[0];
            }
        }

        // 属性更改通知方法
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 清空所有隧道模型、唯一节点模型和过滤后的隧道模型
        public void ClearSuidaoModels()
        {
            SuidaoModels.Clear();
            UniqueNodeModels.Clear();
            FilteredSuidaoModels.Clear();
        }

        // 异步删除指定ID和用户ID的隧道模型
        public async Task DeleteSuidaoModel(int id, int userId)
        {
            var savedLogin = ((App)Application.Current).LoadLoginState();
            var postData = new Dictionary<string, string>
            {
                { "token", savedLogin.Token },
                { "nodeid", id.ToString() },
                { "userid", userId.ToString() }
            };

            try
            {
                var response = await _httpClient.PostAsync($"{BaseApiUrl}/api/deletetl.php", new FormUrlEncodedContent(postData));

                if (!response.IsSuccessStatusCode)
                {
                    ShowErrorMessage($"发送失败，状态码：{response.StatusCode}");
                    return;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var keyValuePairs = JObject.Parse(responseBody);
                var state = keyValuePairs["code"]?.ToString().ToLower();

                var message = state switch
                {
                    "200" => "删除隧道成功",
                    "400" or "error" or "fail" => "删除隧道失败",
                    _ => $"未知的状态码：{state}"
                };

                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"请求发生错误: {ex.Message}");
            }
        }

        // 异步加载隧道数据
        public async Task LoadDataAsync()
        {
            var savedLogin = ((App)Application.Current).LoadLoginState();
            var postData = new Dictionary<string, string> { { "token", savedLogin.Token } };

            try
            {
                var response = await _httpClient.PostAsync($"{BaseApiUrl}/tunnel", new FormUrlEncodedContent(postData));

                if (!response.IsSuccessStatusCode)
                {
                    ShowErrorMessage($"发送失败，状态码：{response.StatusCode}");
                    return;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var keyValuePairs = JObject.Parse(responseBody);
                var state = keyValuePairs["state"]?.ToString().ToLower();

                if (state != "success")
                {
                    ShowErrorMessage(state switch
                    {
                        "fail1" or "error" or "fail" => "获取隧道失败",
                        _ => $"未知的状态码：{state}"
                    });
                    return;
                }

                if (!(keyValuePairs["data"] is JArray dataArray))
                {
                    ShowErrorMessage("返回的数据格式不正确，缺少'data'字段");
                    return;
                }

                SuidaoModels.Clear();

                foreach (var item in dataArray.OfType<JObject>())
                {
                    var suidao = CreateSuidaoModelFromJObject(item);
                    if (suidao != null)
                    {
                        SuidaoModels.Add(suidao);
                    }
                }

                UpdateUniqueNodes();

                if (SuidaoModels.Count > 0)
                {
                    SelectedNode = SuidaoModels[0];
                    SelectedSuidao = SuidaoModels[0];
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"请求发生错误: {ex.Message}");
            }
        }

        // 从 JObject 对象创建 SuidaoModel 对象
        private SuidaoModel CreateSuidaoModelFromJObject(JObject data)
        {
            if (data == null) return null;

            var type = data["type"]?.ToString() ?? string.Empty;
            var address = data["ip"]?.ToString() ?? string.Empty;
            var connectUrl = data["dorp"]?.ToString() ?? string.Empty;

            if (type is "tcp" or "udp")
            {
                connectUrl = $"{address}:{connectUrl}";
            }

            return new SuidaoModel
            {
                ID = data["id"]?.ToObject<int>() ?? 0,
                Name = data["name"]?.ToString() ?? string.Empty,
                LocalIP = data["localip"]?.ToString() ?? string.Empty,
                Type = type,
                NPort = data["nport"]?.ToObject<int>() ?? 0,
                Address = address,
                ConnectUrl = connectUrl,
                Node = data["node"]?.ToString() ?? string.Empty,
                State = data["state"]?.ToString() ?? string.Empty,
                NodeState = data["nodestate"]?.ToString() ?? string.Empty,
                UserID = data["userid"]?.ToObject<int>() ?? 0,
                UseEncryption = bool.TryParse(data["encryption"]?.ToString(), out var useEncryption) && useEncryption,
                UseCompression = bool.TryParse(data["compression"]?.ToString(), out var useCompression) && useCompression,
                AP = data["ap"]?.ToString() ?? string.Empty,
                Uptime = ParseDateTime(data["uptime"]?.ToString()),
                ClientVersion = data["client_version"]?.ToString() ?? string.Empty,
                TodayTrafficIn = data["today_traffic_in"]?.ToObject<long>() ?? 0,
                TodayTrafficOut = data["today_traffic_out"]?.ToObject<long>() ?? 0,
            };
        }

        // 解析日期时间字符串为 DateTime 对象
        private DateTime? ParseDateTime(string dateString)
        {
            return DateTime.TryParse(dateString, out var result) ? result : (DateTime?)null;
        }

        // 显示错误信息
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
