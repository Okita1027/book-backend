using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace book_frontend.Views.UserControls;

public partial class LoadingMask : UserControl
{
    public LoadingMask()
    {
        InitializeComponent();
    }

    /*
     * 是否显示遮罩（绑定到 ViewModel 的 IsLoading）
     */
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public static readonly DependencyProperty IsBusyProperty =
        DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(LoadingMask), new PropertyMetadata(false));

    /*
     * 遮罩背景（通常是半透明黑）
     */
    public Brush MaskBackground
    {
        get => (Brush)GetValue(MaskBackgroundProperty);
        set => SetValue(MaskBackgroundProperty, value);
    }

    public static readonly DependencyProperty MaskBackgroundProperty =
        DependencyProperty.Register(nameof(MaskBackground), typeof(Brush), typeof(LoadingMask),
            new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))));

    /*
     * 容器背景
     */
    public Brush ContainerBackground
    {
        get => (Brush)GetValue(ContainerBackgroundProperty);
        set => SetValue(ContainerBackgroundProperty, value);
    }

    public static readonly DependencyProperty ContainerBackgroundProperty =
        DependencyProperty.Register(nameof(ContainerBackground), typeof(Brush), typeof(LoadingMask),
            new PropertyMetadata(Brushes.White));

    /*
     * 圆角
     */
    public CornerRadius ContainerCornerRadius
    {
        get => (CornerRadius)GetValue(ContainerCornerRadiusProperty);
        set => SetValue(ContainerCornerRadiusProperty, value);
    }

    public static readonly DependencyProperty ContainerCornerRadiusProperty =
        DependencyProperty.Register(nameof(ContainerCornerRadius), typeof(CornerRadius), typeof(LoadingMask),
            new PropertyMetadata(new CornerRadius(8)));

    /*
     * 内边距
     */
    public Thickness ContainerPadding
    {
        get => (Thickness)GetValue(ContainerPaddingProperty);
        set => SetValue(ContainerPaddingProperty, value);
    }

    public static readonly DependencyProperty ContainerPaddingProperty =
        DependencyProperty.Register(nameof(ContainerPadding), typeof(Thickness), typeof(LoadingMask),
            new PropertyMetadata(new Thickness(16)));

    /*
     * 进度条宽高与颜色
     */
    public double ProgressBarWidth
    {
        get => (double)GetValue(ProgressBarWidthProperty);
        set => SetValue(ProgressBarWidthProperty, value);
    }

    public static readonly DependencyProperty ProgressBarWidthProperty =
        DependencyProperty.Register(nameof(ProgressBarWidth), typeof(double), typeof(LoadingMask),
            new PropertyMetadata(150.0));

    public double ProgressBarHeight
    {
        get => (double)GetValue(ProgressBarHeightProperty);
        set => SetValue(ProgressBarHeightProperty, value);
    }

    public static readonly DependencyProperty ProgressBarHeightProperty =
        DependencyProperty.Register(nameof(ProgressBarHeight), typeof(double), typeof(LoadingMask),
            new PropertyMetadata(6.0));

    public Brush ProgressBarForeground
    {
        get => (Brush)GetValue(ProgressBarForegroundProperty);
        set => SetValue(ProgressBarForegroundProperty, value);
    }

    public static readonly DependencyProperty ProgressBarForegroundProperty =
        DependencyProperty.Register(nameof(ProgressBarForeground), typeof(Brush), typeof(LoadingMask),
            new PropertyMetadata(new SolidColorBrush(Color.FromRgb(33, 150, 243))));

    /*
     * 文本样式
     */
    public string LoadingText
    {
        get => (string)GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }

    public static readonly DependencyProperty LoadingTextProperty =
        DependencyProperty.Register(nameof(LoadingText), typeof(string), typeof(LoadingMask),
            new PropertyMetadata("正在加载..."));

    public double TextFontSize
    {
        get => (double)GetValue(TextFontSizeProperty);
        set => SetValue(TextFontSizeProperty, value);
    }

    public static readonly DependencyProperty TextFontSizeProperty =
        DependencyProperty.Register(nameof(TextFontSize), typeof(double), typeof(LoadingMask),
            new PropertyMetadata(14.0));

    public Brush TextForeground
    {
        get => (Brush)GetValue(TextForegroundProperty);
        set => SetValue(TextForegroundProperty, value);
    }

    public static readonly DependencyProperty TextForegroundProperty =
        DependencyProperty.Register(nameof(TextForeground), typeof(Brush), typeof(LoadingMask),
            new PropertyMetadata(Brushes.Black));
}