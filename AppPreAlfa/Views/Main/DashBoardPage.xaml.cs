using AppPreAlfa.ViewModels.Main;
using ReactiveUI;
using Microsoft.UI.Xaml.Controls;

namespace AppPreAlfa.Views.Main;

public sealed partial class DashBoardPage : Page
{
    public DashBoardViewModel ViewModel { get; } = new DashBoardViewModel();

    public DashBoardPage()
    {
        InitializeComponent();
        DataContext = ViewModel;
    }
}
