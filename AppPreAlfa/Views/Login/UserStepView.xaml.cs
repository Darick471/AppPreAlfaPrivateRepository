using AppPreAlfa.ViewModels.Login;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ReactiveUI;
using System.Reactive.Disposables;

namespace AppPreAlfa.Views.Login;

public sealed partial class UserStepView : Page, IViewFor<UserStepViewModel>
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(UserStepViewModel),
            typeof(UserStepView),
            new PropertyMetadata(null, OnViewModelChanged));

    public UserStepViewModel ViewModel
    {
        get => (UserStepViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (UserStepViewModel)value;
    }

    public UserStepView()
    {
        this.InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.BindCommand(
                this.ViewModel,
                vm => vm.SiguienteCommand,
                v => v.SiguienteButton)
            .DisposeWith(disposable);
        });
    }
    private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Page page)
        {
            page.DataContext = e.NewValue;
        }
    }
}
