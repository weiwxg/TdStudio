using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TdStudio.Taos;
using Wpf.Ui.Controls;

namespace TdStudio;

public partial class ConnectWindow : UiWindow
{
    public ConnectWindow()
    {
        InitializeComponent();
    }

    private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
    {
        ConnectMessage.Visibility = Visibility.Collapsed;
        Loading.Visibility = Visibility.Visible;

        App.TaosConnector = new TaosConnector(TxtConnection.Text);

        try
        {
            await App.TaosConnector.ToDataTableAsync(@"show stables");
            Hide();
            new MainWindow().Show();
        }
        catch
        {
            ConnectMessage.Visibility = Visibility.Visible;
        }
        finally
        {
            Loading.Visibility = Visibility.Collapsed;
        }
    }
}