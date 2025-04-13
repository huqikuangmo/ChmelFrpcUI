using FrpcUI;
using FrpcUI.Class;

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UIKitTutorials.Pages
{
    /// <summary>
    /// Lógica de interacción para NotesPage.xaml
    /// </summary>
    public partial class SuiDaoPage : Page
    {
        public SuidaoViewModel viewModel = new SuidaoViewModel();
        
        private readonly HttpClient httpClient;

        public SuiDaoPage()
        {
            
            httpClient = new HttpClient();
            InitializeComponent();
            this.DataContext = viewModel; // 修改为使用正确的ViewModel类型

            // 调用并等待 LoadDataAsync 方法执行完成
            _= viewModel.LoadDataAsync();

        }

        public void refresh_Click(object sender, RoutedEventArgs e)
        {

            viewModel.ClearSuidaoModels(); // 调用 ViewModel 中的方法来清除集合内容
            _ = viewModel.LoadDataAsync(); // 重新加载数据
        }

        public void add_Click(object sender, RoutedEventArgs e)
        {
            // 实例化Window
            Tianjiasuidao tianjiasuiddao = new Tianjiasuidao();
            tianjiasuiddao.Show();
        }

        private void Token_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.Text != null)
            {
                Clipboard.SetText(textBlock.Text);
            }
        }

        public async void delete_Click(object sender, RoutedEventArgs e)
        {
            // 获取点击的按钮
            Button button = sender as Button;

            // 获取按钮所在的 StackPanel
            StackPanel stackPanel = button.Parent as StackPanel;

            // 获取 StackPanel 的 DataContext，即节点的数据对象
            var suidao = stackPanel.DataContext as SuidaoModel;
            // 如果获取到了数据对象，则获取 Name 属性的值
            if (suidao != null)
            {
                await viewModel.DeleteSuidaoModel(suidao.ID, suidao.UserID); // 调用 ViewModel 中的方法来删除数据对象
                viewModel.ClearSuidaoModels(); // 调用 ViewModel 中的方法来清除集合内容
                _ = viewModel.LoadDataAsync(); // 重新加载数据
            }
            else
            {
                MessageBox.Show("未找到节点数据对象。");
            }
        }
    }
}
