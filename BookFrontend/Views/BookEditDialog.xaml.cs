using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using book_frontend.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace book_frontend.Views
{
    /// <summary>
    /// BookEditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class BookEditDialog : Window
    {
        private readonly BookEditViewModel _viewModel;

        public BookEditDialog()
        {
            InitializeComponent();
            
            // 从依赖注入容器获取ViewModel
            _viewModel = App.ServiceProvider.GetRequiredService<BookEditViewModel>();
            DataContext = _viewModel;
        }

        /// <summary>
        /// 显示新增图书对话框
        /// </summary>
        /// <param name="owner">父窗口</param>
        /// <returns>对话框结果</returns>
        public static async Task<bool?> ShowAddDialogAsync(Window owner)
        {
            var dialog = new BookEditDialog
            {
                Owner = owner
            };

            // 初始化为新增模式
            await dialog._viewModel.InitializeAsync();
            
            return dialog.ShowDialog();
        }

        /// <summary>
        /// 显示编辑图书对话框
        /// </summary>
        /// <param name="owner">父窗口</param>
        /// <param name="bookId">要编辑的图书ID</param>
        /// <returns>对话框结果</returns>
        public static async Task<bool?> ShowEditDialogAsync(Window owner, int bookId)
        {
            var dialog = new BookEditDialog
            {
                Owner = owner
            };

            // 初始化为编辑模式
            await dialog._viewModel.InitializeAsync(bookId);
            
            return dialog.ShowDialog();
        }

        /// <summary>
        /// 数字输入框的输入验证
        /// </summary>
        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 只允许输入数字
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// 窗口加载完成事件
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置焦点到第一个输入框
            var firstTextBox = FindVisualChild<System.Windows.Controls.TextBox>(this);
            firstTextBox?.Focus();
        }

        /// <summary>
        /// 查找可视化树中的子元素
        /// </summary>
        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                {
                    return tChild;
                }

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 如果正在保存，阻止关闭
            if (_viewModel.IsSaving)
            {
                e.Cancel = true;
                MessageBox.Show("正在保存中，请稍候...", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}