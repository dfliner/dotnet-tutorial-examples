using System.Windows;
using WpfAppDI.StartupHelper;
using WpfAppDI.ViewModel;

namespace WpfAppDI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IAbstractFactory<ChildWindow> _factory;

    public MainWindow(UserViewModel viewModel, IAbstractFactory<ChildWindow> factory)
    {
        _factory = factory;
        DataContext = viewModel;

        InitializeComponent();
    }

    private void BtnOpenChildWindow_Click(object sender, RoutedEventArgs e)
    {
        var childWindow = _factory.Create();
        childWindow.Show();
    }
}
