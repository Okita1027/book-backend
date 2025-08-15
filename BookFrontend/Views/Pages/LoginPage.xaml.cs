using System.Windows;
using System.Windows.Controls;
using book_frontend.ViewModels;

namespace book_frontend.Views.Pages
{
    // 该视图只负责界面展示与绑定，不在代码后置中设置 DataContext。
    // DataContext 将在下一步（功能点 4：依赖注入与启动配置）统一注入。
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.TriggerNavigateToRegister();
            }
        }
    }
}