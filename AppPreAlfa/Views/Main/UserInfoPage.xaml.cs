using AppPreAlfa.ViewModels.Main;
using ReactiveUI;
using Microsoft.UI.Xaml.Controls;

namespace AppPreAlfa.Views.Main;

public sealed partial class UserInfoPage : Page, IViewFor<UserInfoViewModel>
{
    public UserInfoViewModel ViewModel { get; set; }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (UserInfoViewModel)value;
    }

    public UserInfoPage()
    {
        InitializeComponent();
        ViewModel = new UserInfoViewModel();
        DataContext = ViewModel;
    }
}
