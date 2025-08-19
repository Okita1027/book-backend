using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace book_frontend.Models.VOs;

public partial class CategoryVO : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}