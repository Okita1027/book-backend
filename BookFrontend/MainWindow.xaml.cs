using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using book_frontend.ViewModels;
using book_frontend.Views.UserControls;

namespace book_frontend;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 设置登录页面的DataContext
    /// </summary>
    /// <param name="loginViewModel">登录视图模型</param>
    public void SetLoginViewModel(LoginViewModel loginViewModel)
    {
        // 找到LoginView控件并设置其DataContext
        /*
         * Content 是当前窗口的内容对象
         * is Grid grid 使用C#的模式匹配语法，检查Content是否为Grid类型
         * 如果是Grid类型，则将Content转换为Grid类型并赋值给新变量grid
         */
        if (Content is Grid grid && grid.Children[0] is LoginView loginView)
        {
            loginView.DataContext = loginViewModel;
        }
    }
}