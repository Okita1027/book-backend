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

namespace book_frontend;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // 创建一个TextBlock来显示Hello World消息
        TextBlock textBlock = new TextBlock();
        textBlock.Text = "Hello, World!";
        textBlock.FontSize = 24;
        textBlock.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock.VerticalAlignment = VerticalAlignment.Center;

        // 将TextBlock添加到窗口的内容中
        this.Content = textBlock;
    }
}