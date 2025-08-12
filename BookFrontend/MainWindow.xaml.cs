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
using book_frontend.Views;

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
    /// 设置主ViewModel的DataContext
    /// </summary>
    /// <param name="mainViewModel">主视图模型</param>
    public void SetMainViewModel(MainViewModel mainViewModel)
    {
        this.DataContext = mainViewModel;
        
        // 找到HomeView控件并设置其DataContext为MainViewModel.Home
        if (Content is DockPanel dockPanel)
        {
            foreach (var child in dockPanel.Children)
            {
                if (child is Grid grid)
                {
                    foreach (var gridChild in grid.Children)
                    {
                        if (gridChild is HomeView homeView)
                        {
                            homeView.DataContext = mainViewModel.Home;
                            break;
                        }
                    }
                }
            }
        }
    }
}