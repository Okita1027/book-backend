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
            if (DataContext is not BookListViewModel viewModel)
            {
                return;
            }

            if (sender is not ScrollViewer scrollViewer)
            {
                return;
            }

            const double threshold = 100;

            /*
             * ExtentHeight：可滚动内容的总高度（包括不可见部分）
             *      代表整个内容区域的高度
             *      包括当前可见区域和需要滚动才能看到的区域
             * ViewportHeight：可见区域的高度（视口高度）
             *      代表当前用户能看到的内容区域高度
             *      即ScrollViewer控件显示内容的区域高度
             * 如果内容高度或视口高度发生变化（如窗口最大化/还原、数据加载后列表变高），
             * 尝试预取，直到出现可滚动高度或达到一定次数上限。
             */
            var sizeChanged = e.ExtentHeightChange != 0 || e.ViewportHeightChange != 0;
            if (sizeChanged)
            {
                var prefetchCount = 0;
                while (prefetchCount < 3 && viewModel is { IsLoading: false, HasNextPage: true })
                {
                    /*
                     * ScrollableHeight 表示可滚动的垂直距离：
                     *      它是内容总高度与可见区域高度的差值
                     *      计算公式：ScrollableHeight = ExtentHeight - ViewportHeight
                     *      当内容高度小于或等于可见区域高度时，值为 0
                     *      表示用户最多可以向下滚动多少距离
                     * VerticalOffset 表示当前垂直滚动的偏移量：
                     *      它表示用户已经向下滚动了多少距离
                     *      从顶部开始计算，0表示在最顶部
                     *      最大值等于ScrollableHeight（滚动到底部时）
                     */
                    var remainingSize = scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset;
                    if (scrollViewer.ScrollableHeight == 0 || remainingSize <= threshold)
                    {
                        await viewModel.LoadNextPageAsync();
                        prefetchCount++;
                        /*
                         * 向UI线程中添加一个（优先级最低的）啥也不干的后台线程
                         * 当这个空操作被执行，说明所有的UI更新已经完成了，可以开始执行下一轮的条件判断、数据加载了
                         */
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
                var remaining = scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset;
                if (remaining <= threshold)
                {
                    await viewModel.LoadNextPageAsync();
                }
            }
        }
    }
}