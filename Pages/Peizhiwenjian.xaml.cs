using FrpcUI.Class;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrpcUI.Pages
{
    /// <summary>
    /// Peizhiwenjian.xaml 的交互逻辑
    /// </summary>
    public partial class Peizhiwenjian : Page
    {
        private object nodeValue;
        public SuidaoViewModel viewModel = new SuidaoViewModel();
        public async void InitializeData()
        {
            await viewModel.LoadDataAsync();
        }
        public Peizhiwenjian()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            InitializeData(); // 显式调用异步加载
        }

        public void ClearName_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var grid = button.Parent as Grid;
            var textBlock = grid?.FindName("suidao") as TextBlock;

            if (textBlock != null)
            {
                textBlock.Text = string.Empty;

                if (nodeComboBox.SelectedItem is SuidaoModel selectedNode)
                {
                    selectedNode.Name = string.Empty; // 更新数据模型
                }
            }
            
        }

        public void Shuaxing_Click(object sender, RoutedEventArgs e)
        {
            InitializeData();
        }

        public async void Shencheng_Click(object sender, RoutedEventArgs e)
        {
            if (nodeComboBox.SelectedItem is SuidaoModel SelectedNode)
            {
                string nodeValue = SelectedNode.Node;
                string nameValue = SelectedNode.Name;
                PeizhiModel peizhi = new PeizhiModel(nodeValue, nameValue);
                string configData = await peizhi.LoadDataAsync();
                configText.Text = configData;
            }
        }



        public void Write_Click(object sender, RoutedEventArgs e)
        {
            if (nodeComboBox.SelectedItem is SuidaoModel SelectedNode)
            {
                nodeValue = SelectedNode.Node;
            }

            try
            {

                // 获取要保存的文本内容
                string content = configText.Text;

                // 如果内容为空则提示
                if (string.IsNullOrWhiteSpace(content))
                {
                    MessageBox.Show("配置内容为空，无法保存", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string targetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Frpc");

                // 生成文件名
                string fileName = $"frpc_config_{nodeValue}.ini";
                string fullPath = Path.Combine(targetFolder, fileName);

                // 写入文件
                File.WriteAllText(fullPath, content);

                // 提示保存成功
                MessageBox.Show($"配置已保存到:\n{fullPath}", "保存成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("无权限写入指定目录", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"文件写入错误:\n{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生未知错误:\n{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
