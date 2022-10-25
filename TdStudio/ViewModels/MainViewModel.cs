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
    private string _dbName;
    private ObservableCollection<Stable> _stables;
    private DataTable _data;
    private string _errorMessage;

    public MainViewModel()
    {
        _stables = new ObservableCollection<Stable>();
        _data = null;
        _errorMessage = string.Empty;
    }

    public string DbName
    {
        get => _dbName;
        set => SetField(ref _dbName, value);
    }

    public ObservableCollection<Stable> Stables
    {
        get => _stables;
        set => SetField(ref _stables, value);
    }

    public DataTable Data
    {
        get => _data;
        set => SetField(ref _data, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
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