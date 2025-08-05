using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AppPreAlfa;

public sealed partial class MainWindow : Window
{
    // Acceso público al Frame
    public Frame MainFrame => mainFrame;

    public MainWindow()
    {
        this.InitializeComponent();
    }
}
