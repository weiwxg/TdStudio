using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TdStudio.Models;
using TdStudio.Taos;
using TdStudio.ViewModels;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace TdStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            App.TaosConnector!.ToListAsync<Stable>("show stables")
                .ContinueWith(t =>
                {
                    _viewModel.DbName = App.TaosConnector.DbName;
                    _viewModel.Stables = new ObservableCollection<Stable>(t.Result!);
                });
        }

        private async void BtnRun_OnClick(object sender, RoutedEventArgs e)
        {
            (sender as Button)!.IsEnabled = false;
            _viewModel.ErrorMessage = string.Empty;
            _viewModel.Data = new DataTable();

            var sql = QueryBox.SelectedText.Trim().Length > 0
                ? QueryBox.SelectedText
                : QueryBox.Text;

            if (string.IsNullOrWhiteSpace(sql))
            {
                (sender as Button)!.IsEnabled = true;
                return;
            }

            try
            {
                var dt = await App.TaosConnector!.ToDataTableAsync(sql.Trim());
                _viewModel.Data = dt;
            }
            catch (Exception ex)
            {
                _viewModel.ErrorMessage = ex.Message;
            }
            finally
            {
                (sender as Button)!.IsEnabled = true;
            }
        }

        private void DataGrid_OnAutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // 处理自动生成列对于DateTime类型的序列化自定义问题
            if (e.PropertyType == typeof(DateTime))
            {
                var header = e.Column.Header;
                e.Column = new DataGridTextColumn
                {
                    Binding = new Binding(e.PropertyName) { StringFormat = "O" }
                };
                e.Column.Header = header;
            }
        }
    }
}