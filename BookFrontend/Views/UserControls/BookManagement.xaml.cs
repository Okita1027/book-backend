using System.Windows.Controls;
using book_frontend.ViewModels;
using book_frontend.Models.VOs;
using System.Collections.Specialized;
using System.Windows;

namespace book_frontend.Views.UserControls
{
    /// <summary>
    /// BookManagement.xaml 的交互逻辑
    /// </summary>
    public partial class BookManagement : UserControl
    {
        private readonly BookManagementViewModel _viewModel;

        public BookManagement(BookManagementViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // 处理DataGrid选择变化
            BooksDataGrid.SelectionChanged += BooksDataGrid_SelectionChanged;
        }

        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.SelectedBooks.Clear();
            foreach (BookVOWrapper item in BooksDataGrid.SelectedItems)
            {
                _viewModel.SelectedBooks.Add(item);
            }
        }
    }
}