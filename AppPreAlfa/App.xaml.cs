using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AppPreAlfa.Activation;
using AppPreAlfa.Views.Login;
using ReactiveUI;
using Splat;

namespace AppPreAlfa;

public partial class App : Application
{
    public MainWindow MainWindow { get; private set; }
    private IScreen _screen;

    public App()
    {
        this.InitializeComponent();
        Locator.CurrentMutable.RegisterConstant(new WinUIActivationForViewFetcher(), typeof(IActivationForViewFetcher));
        _screen = new MainScreen();
        Locator.CurrentMutable.RegisterConstant(_screen, typeof(IScreen));
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();

        if (MainWindow.Content is not Frame frame)
        {
            frame = new Frame();
            MainWindow.Content = frame; 
        }

        if (frame.Content == null)
        {
            frame.Navigate(typeof(LoginPage));
        }

        MainWindow.Activate();
    }
}
