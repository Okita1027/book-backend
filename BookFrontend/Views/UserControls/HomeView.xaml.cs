using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using book_frontend.ViewModels;
using System.Windows.Threading;

namespace book_frontend.Views.UserControls
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 回车搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is BookListViewModel viewModel)
            {
                if (viewModel.SearchCommand.CanExecute(null))
                {
                    viewModel.SearchCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// 列表滚动事件：接近底部时自动加载下一页。
        /// 同时在内容尺寸或视口尺寸变化（例如窗口最大化）时，尝试预取数据以填满视口，避免“没有可滚动高度”导致的触发失效。
        /// </summary>
        private async void BookListScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (DataContext is not BookListViewModel vm) return;

            var sv = sender as ScrollViewer;
            if (sv == null) return;

            const double threshold = 100; // px

            // 如果是内容高度或视口高度发生变化（如窗口最大化/还原、数据加载后列表变高），
            // 尝试预取，直到出现可滚动高度或达到一定次数上限。
            bool sizeChanged = e.ExtentHeightChange != 0 || e.ViewportHeightChange != 0;
            if (sizeChanged)
            {
                int prefetchCount = 0;
                while (prefetchCount < 3 && !vm.IsLoading && vm.HasNextPage)
                {
                    var remainingSize = sv.ScrollableHeight - sv.VerticalOffset;
                    if (sv.ScrollableHeight == 0 || remainingSize <= threshold)
                    {
                        await vm.LoadNextPageAsync();
                        prefetchCount++;
                        // 让UI有机会刷新布局，获取最新的 ScrollableHeight
                        await Application.Current.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);
                    }
                    else
                    {
                        break;
                    }
                }

                // 尺寸变化场景已处理，直接返回，避免下面的“仅向下滚动”逻辑误判
                return;
            }

            // 仅在向下滚动并接近底部时加载下一页
            if (e.VerticalChange > 0)
            {
                var remaining = sv.ScrollableHeight - sv.VerticalOffset;
                if (remaining <= threshold)
                {
                    await vm.LoadNextPageAsync();
                }
            }
        }
    }
}