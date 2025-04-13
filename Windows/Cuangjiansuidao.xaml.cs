using FrpcUI.Class;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrpcUI.Windows
{
    /// <summary>
    /// Cuangjiansuidao.xaml 的交互逻辑
    /// </summary>
    public partial class Cuangjiansuidao : Window
    {
        public string SelectedNode { get; set; }
        public string Jianzhan { get; set; }

        public Cuangjiansuidao(string name, string jianzhan)
        {
            InitializeComponent();
            SelectedNode = name;
            Jianzhan = jianzhan;
            DataContext = this;
        }

        private void ComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var newWindow = new Tianjiasuidao();
            newWindow.Show();
            this.Close();
        }

        private bool _isInitializing = true;
        private object sender;

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            _isInitializing = false; // 标记初始化完成
            PortType_SelectionChanged(sender, null); // 初始化调用
        }

        private void PortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing)
                return; // 如果仍在初始化，退出方法

            var selectedItem = PortType.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string portType = selectedItem.Content.ToString();
                if (portType == "HTTP" || portType == "HTTPS")
                {
                    Yuming.IsReadOnly = false;
                    OuterPort.IsReadOnly = true;

                }
                else if (portType == "UDP" || portType == "TCP")
                {
                    Yuming.IsReadOnly = true;
                    OuterPort.IsReadOnly = false;

                }
            }
        }




        public void Queding_Click(object sender, RoutedEventArgs e)
        {
            string portType = PortType.Text;//获取选择的端口类型
            if (Jianzhan == "不可建站" && (portType == "HTTP" || portType == "HTTPS"))
            {
                MessageBox.Show("该节点不允许建站，请重新选择可建站节点！");
            }
            else
            {
                string tunnelName = TunnelName.Text;//获取输入的隧道名称
                string innerPort = InnerPort.Text;//获取输入的内网端口
                string localIp = LocalIP.Text; // 获取输入的本地IP

                string outerPort = OuterPort.Text;//获取输入的外网端口
                string yuming = Yuming.Text;//获取输入的域名
                                            // 获取数据加密CheckBox的选中状态
                bool shujujiami = DataEncryptionCheckBox.IsChecked ?? false;

                // 获取数据压缩CheckBox的选中状态
                bool shujuyasuo = DataCompressionCheckBox.IsChecked ?? false;
                string extraParams = ExtraParams.Text;
                CuangjianViewModel viewModel = new CuangjianViewModel(tunnelName, innerPort, localIp, portType, outerPort, SelectedNode, yuming, shujujiami, shujuyasuo, extraParams);
                string result = viewModel.Status();
                if (result == "True")
                {
                    this.Close();
                }
                //else
                //{
                //    Tianjiasuidao2 tianjiasuidao2 = new Tianjiasuidao2(SelectedNode);
                //    tianjiasuidao2.Show();
                //    this.Close();
                //}
            }

        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Previous_Click(object sender, RoutedEventArgs e)
        {
            Tianjiasuidao2 tianjiasuidao2 = new Tianjiasuidao2(SelectedNode);
            tianjiasuidao2.Show();
            this.Close();
        }

        public void RandomName_Click(object sender, RoutedEventArgs e)
        {
            TunnelName.Text = GenerateRandomString(8);
        }

        private static readonly Random _random = new Random();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private string GenerateRandomString(int length)
        {
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Chars[_random.Next(Chars.Length)];
            }
            return new string(result);
        }

        public void RandomOuterPort_Click(object sender, RoutedEventArgs e)
        {
            OuterPort.Text = new Random().Next(10000, 65535).ToString();
        }
    }
}
