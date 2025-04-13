using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UIKitTutorials;

namespace FrpcUI.Class
{
    /// <summary>
    /// 隧道视图模型，处理UI逻辑和数据绑定
    /// </summary>
    public class SuidaoViewModel : INotifyPropertyChanged
    {
        private readonly SuidaoService _suidaoService;
        private SuidaoModel _selectedNode;
        private SuidaoModel _selectedSuidao;
        private ObservableCollection<SuidaoModel> _uniqueNodeModels;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<SuidaoModel> SuidaoModels { get; }
        public ObservableCollection<SuidaoModel> UniqueNodeModels
        {
            get => _uniqueNodeModels;
            private set
            {
                _uniqueNodeModels = value;
                OnPropertyChanged(nameof(UniqueNodeModels));
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
                    UpdateSelectedSuidao();
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

        public ICommand ClearNodeCommand { get; }
        public ICommand DeleteSuidaoCommand { get; }

        public SuidaoViewModel()
        {
            _suidaoService = new SuidaoService();
            SuidaoModels = new ObservableCollection<SuidaoModel>();
            UniqueNodeModels = new ObservableCollection<SuidaoModel>();

            ClearNodeCommand = new RelayCommand<SuidaoModel>(ClearNode);
            DeleteSuidaoCommand = new RelayCommand<int>(DeleteSuidao);
        }

        /// <summary>
        /// 加载隧道数据
        /// </summary>
        public async Task LoadDataAsync()
        {
            var savedLogin = ((App)Application.Current).LoadLoginState();
            var response = await _suidaoService.LoadTunnelDataAsync(savedLogin.Token);

            if (response == null) return;

            string state = response["state"]?.ToString().ToLower() ?? string.Empty;

            switch (state)
            {
                case "success":
                    ProcessSuccessResponse(response);
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

        /// <summary>
        /// 清除选中的节点
        /// </summary>
        private void ClearNode(SuidaoModel node)
        {
            if (SelectedNode == node)
            {
                SelectedNode = null;
            }
        }

        /// <summary>
        /// 删除隧道
        /// </summary>
        private async void DeleteSuidao(int id)
        {
            var savedLogin = ((App)Application.Current).LoadLoginState();
            await _suidaoService.DeleteTunnelAsync(id, savedLogin.UserID, savedLogin.Token);
            await LoadDataAsync(); // 刷新数据
        }

        /// <summary>
        /// 处理成功的API响应
        /// </summary>
        private void ProcessSuccessResponse(JObject response)
        {
            var dataArray = response["data"] as JArray;
            if (dataArray == null)
            {
                MessageBox.Show("返回的数据格式不正确，缺少'data'字段");
                return;
            }

            SuidaoModels.Clear();
            foreach (var item in dataArray)
            {
                var suidao = CreateSuidaoModelFromJToken(item);
                if (suidao != null)
                {
                    SuidaoModels.Add(suidao);
                }
            }

            UpdateUniqueNodes();
            SetDefaultSelections();
        }

        /// <summary>
        /// 从JToken创建隧道模型
        /// </summary>
        private SuidaoModel CreateSuidaoModelFromJToken(JToken item)
        {
            var data = item as JObject;
            if (data == null) return null;

            string type = data["type"]?.ToString() ?? string.Empty;
            string address = data["ip"]?.ToString() ?? string.Empty;
            string connectUrl = data["dorp"]?.ToString() ?? string.Empty;

            switch (type)
            {
                case "tcp":
                case "udp":
                    connectUrl = $"{address}:{connectUrl}";
                    break;
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
                UseEncryption = bool.TryParse(data["encryption"]?.ToString(), out bool useEncryption) && useEncryption,
                UseCompression = bool.TryParse(data["compression"]?.ToString(), out bool useCompression) && useCompression,
                AP = data["ap"]?.ToString() ?? string.Empty,
                Uptime = ParseDateTime(data["uptime"]),
                ClientVersion = data["client_version"]?.ToString() ?? string.Empty,
                TodayTrafficIn = data["today_traffic_in"]?.ToObject<long>() ?? 0,
                TodayTrafficOut = data["today_traffic_out"]?.ToObject<long>() ?? 0,
            };
        }

        /// <summary>
        /// 解析日期时间
        /// </summary>
        private DateTime? ParseDateTime(JToken token)
        {
            if (token == null) return null;
            return DateTime.TryParse(token.ToString(), out DateTime result) ? result : (DateTime?)null;
        }

        /// <summary>
        /// 更新唯一节点列表
        /// </summary>
        private void UpdateUniqueNodes()
        {
            var uniqueNodes = SuidaoModels
                .GroupBy(x => x.Node)
                .Select(g => g.First())
                .ToList();

            UniqueNodeModels = new ObservableCollection<SuidaoModel>(uniqueNodes);
        }

        /// <summary>
        /// 设置默认选中的节点和隧道
        /// </summary>
        private void SetDefaultSelections()
        {
            if (SuidaoModels.Count > 0)
            {
                SelectedNode = SuidaoModels[0];
                SelectedSuidao = SuidaoModels[0];
            }
        }

        /// <summary>
        /// 根据选中的节点更新选中的隧道
        /// </summary>
        private void UpdateSelectedSuidao()
        {
            SelectedSuidao = SelectedNode != null
                ? SuidaoModels.FirstOrDefault(x => x.Node == SelectedNode.Node)
                : null;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}