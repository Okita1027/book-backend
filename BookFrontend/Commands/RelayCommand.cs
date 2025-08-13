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
    public void RaiseCanExecuteChanged()
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