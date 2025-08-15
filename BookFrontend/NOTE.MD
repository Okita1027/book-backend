# 项目笔记
## RelayCommand
```csharp
using System.Windows.Input;

namespace book_frontend.Commands;

/// <summary>
/// 用于将任意的执行逻辑（Action)与可执行判断逻辑(Func)包装为ICommand
/// 让View可以通过Command绑定来触发ViewModel中的操作
/// </summary>
public class RelayCommand : ICommand
{
    // 要执行的逻辑（按钮点击之后要做什么）
    private readonly Action<object?> _execute;
    // 判断命令是否可以执行（按钮是否可用）
    private readonly Func<object?, bool>? _canExecute;
    
    /// <summary>
    /// 当命令可执行状态发生改变时触发。
    ///  WPF会订阅这个事件，在需要时刷新按钮的IsEnabled状态
    ///  这里将事件转发给CommandManager.RequerySuggested,
    ///  当输入焦点变化、键盘/鼠标事件发生时，WPF会自动触发CanExecute重新评估
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="execute">执行逻辑，不能为空</param>
    /// <param name="canExecute">是否可以执行，可以为空：代表始终可以执行</param>
    /// <exception cref="ArgumentNullException">当execute为空时抛出</exception>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
    /// <summary>
    /// 命令是否可以执行
    /// 例如：登录按钮在用户名或密码为空时不允许点击
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
        // return _canExecute?.Invoke(parameter) ?? true;
    }
    
    /// <summary>
    /// 执行命令
    /// 例如：调用AuthService进行登录请求
    /// </summary>
    /// <param name="parameter"></param>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
    
    /// <summary>
    /// 主动触发命令的可执行状态变化通知
    /// 当ViewModel的相关属性改变之后（影响CanExecute的逻辑）
    /// 可以调用此方法来更新按钮的可用状态
    /// </summary>
    public static void RaiseCanExecuteChanged()
    {
        // 通知WPF重新评估CanExecute
        CommandManager.InvalidateRequerySuggested();
    }
}

/// <summary>
/// 带参数类型的命令实现（泛型版本）
/// 优点：可以直接得到强类型的参数，而不需要手动转换object
/// </summary>
/// <typeparam name="T">命令参数类型</typeparam>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentException(null, nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        // 安全转换得到T类型
        if (parameter == null && typeof(T).IsValueType)
        {
            // 值类型的null无意义，直接判断逻辑（通常返回false）
            return _canExecute?.Invoke(default) ?? true;
        }
        return _canExecute?.Invoke((T?)parameter) ?? true;
    }

    public void Execute(object? parameter)
    {
        if (parameter == null && typeof(T).IsValueType)
        {
            _execute(default);
            return;
        }
        _execute((T?)parameter);
    }
}
```

## LoginView.xaml

```XAML
<UserControl x:Class="book_frontend.Views.UserControls.LoginView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:materialDesign="http://materialdesigninxaml.net/winfx/xaml/themes"
             xmlns:h="clr-namespace:book_frontend.Helpers"
             xmlns:viewModels="clr-namespace:book_frontend.ViewModels"
             mc:Ignorable="d"
             d:DesignHeight="480" d:DesignWidth="800"
             d:DataContext="{d:DesignInstance Type=viewModels:LoginViewModel, IsDesignTimeCreatable=False}">
```

定义命名空间：

* 默认命名空间是WPF核心命名空间
* x 是XAML语言命名空间
* d 是设计时命名空间（用于设计时支持）
* mc 是标记兼容性命名空间
* materialDesign 是MaterialDesignThemes库的命名空间
* h 是项目中Helpers命名空间的映射
* viewModels 是项目中ViewModels命名空间的映射
* mc:Ignorable="d" 表示d命名空间下的属性在运行时可以被忽略
* d:DesignHeight 和 d:DesignWidth 设置设计时的高度和宽度
* d:DataContext 设置设计时的数据上下文为LoginViewModel（设计时无法创建实例）



---



```XAML
<PasswordBox Grid.Row="4"
                materialDesign:HintAssist.Hint="密码"
                Style="{StaticResource MaterialDesignOutlinedPasswordBox}"
                h:PasswordBoxHelper.BindPassword="True"
                h:PasswordBoxHelper.BoundPassword="{Binding Password, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
                Margin="0,0,0,8"/>
```



---



```xaml
<TextBlock Grid.Row="6"
           Text="{Binding ErrorMessage}"
           Foreground="{DynamicResource MaterialDesignValidationErrorBrush}"
           Visibility="{Binding ErrorMessage, Converter={StaticResource BooleanToVisibilityConverter}, ConverterParameter=Invert}"
           TextWrapping="Wrap"/>
```
- Grid.Row="6"：指定该TextBlock位于父Grid的第6行
- `Text="{Binding ErrorMessage}"`：
  - 绑定到ViewModel的`ErrorMessage`属性
  - 当`ErrorMessage`属性值变化时，TextBlock显示的内容会自动更新
- `Foreground="{DynamicResource MaterialDesignValidationErrorBrush}"`：
  - 使用动态资源设置前景色（文字颜色）
  - 这里使用了MaterialDesign主题包中的验证错误颜色刷子
  - `DynamicResource`允许在运行时更改资源
- `Visibility="{Binding ErrorMessage, Converter={StaticResource BooleanToVisibilityConverter}, ConverterParameter=Invert}"`：
  - 绑定到同一个`ErrorMessage`属性
  - 使用了`BooleanToVisibilityConverter`转换器
  - `ConverterParameter="Invert"`表示反转逻辑：
    - 当`ErrorMessage`为空/null时：控件可见
    - 当`ErrorMessage`有值时：控件隐藏
  - 这可能是为了在其他地方显示错误消息，而这里作为占位符
- `TextWrapping="Wrap"`：设置文本在达到控件边界时自动换行



---



~~~xaml
<Button Grid.Row="8"
        Content="登录"
        Command="{Binding LoginCommand}"
        Style="{StaticResource MaterialDesignRaisedSecondaryButton}"
        HorizontalAlignment="Stretch"
        Height="36"/>
~~~

- `Command="{Binding LoginCommand}"`：
  - 绑定到ViewModel的`LoginCommand`属性（通常类型为`ICommand`）
  - 点击按钮时会执行该命令
  - 命令是否可用取决于`LoginCommand.CanExecute`的返回值

- `HorizontalAlignment="Stretch"`：
  - 按钮水平方向拉伸填充父容器
  - 与Grid结合使用时通常会填满所在单元格的宽度



---



~~~xaml
<Grid Background="#80000000"
      Visibility="{Binding IsLoading, Converter={StaticResource BooleanToVisibilityConverter}}">

    <Border Width="220" Padding="16" CornerRadius="8" Background="{DynamicResource MaterialDesignPaper}">
        <StackPanel>
            <TextBlock Text="正在登录..."
                       HorizontalAlignment="Center"
                       Margin="0,0,0,8"/>
            <ProgressBar IsIndeterminate="True"
                         Foreground="{DynamicResource PrimaryHueMidBrush}"
                         Height="6"/>
        </StackPanel>
    </Border>
</Grid>
~~~

- `Background="#80000000"`：
  - 设置半透明黑色背景(ARGB格式)
  - `#80`表示50%透明度(十六进制128/255)
  - `#000000`表示黑色
- `Visibility`绑定：
  - 绑定到`IsLoading`布尔属性
  - 使用`BooleanToVisibilityConverter`转换器
  - 当`IsLoading=true`时显示，`false`时隐藏





