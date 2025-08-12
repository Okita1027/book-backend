# 项目笔记

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



