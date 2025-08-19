using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace book_frontend.ViewModels;

/// <summary>
/// 这是一个典型的、广泛使用的实现方式
/// ViewModel基类，实现INotifyPropertyChanged接口
/// 这是WPF数据绑定的核心机制，类似于React的useState
/// </summary>
public class BaseViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// 当属性值改变时触发的事件
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// 属性变更通知方法：当属性值改变时调用，通知UI更新
    /// </summary>
    /// <param name="propertyName">
    /// 属性名（自动获取）
    /// [CallerMemberName]是一个编译器特性，会自动将调用此方法的成员名称作为参数传入
    /// </param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 设置属性值的辅助方法
    /// 只有当值真正改变时才触发通知
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    /// <param name="field">ref T field表示传入字段的引用，这样可以直接修改字段的值</param>
    /// <param name="value">新的值</param>
    /// <param name="propertyName">属性名称</param>
    /// <returns>是否发生了改变</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        // 比较新旧值是否相等，如果相等则不触发通知
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }
        // 如果值不同，则更新字段值，调用OnPropertyChanged方法通知UI，并返回true表示值已更改
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
