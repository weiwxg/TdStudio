using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using TdStudio.Models;

namespace TdStudio.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
#nullable disable
    private ObservableCollection<Stable> _stables;
    private DataTable _data;
    private Visibility _dataGridVisibility;

    public MainViewModel()
    {
        _stables = new ObservableCollection<Stable>();
        _data = null;
        _dataGridVisibility = Visibility.Hidden;
    }

    public ObservableCollection<Stable> Stables
    {
        get => _stables;
        set => SetField(ref _stables, value);
    }

    public DataTable Data
    {
        get => _data;
        set
        {
            SetField(ref _data, value);
            DataGridVisibility = value != null && value.Rows.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }
    }

    private Visibility DataGridVisibility
    {
        set => SetField(ref _dataGridVisibility, value);
    }

#nullable enable
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
}