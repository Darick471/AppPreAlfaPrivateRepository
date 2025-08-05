using Microsoft.UI.Xaml.Controls;
using ReactiveUI;
using System.Reactive.Disposables;
using AppPreAlfa.ViewModels.Login;

namespace AppPreAlfa.Views.Login;

public sealed partial class PasswordStepView : Page, IViewFor<PasswordStepViewModel>
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty
        .Register(nameof(ViewModel), typeof(PasswordStepViewModel), typeof(PasswordStepView), new PropertyMetadata(null));

    public PasswordStepViewModel ViewModel
    {
        get => (PasswordStepViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (PasswordStepViewModel)value;
    }

    public PasswordStepView()
    {
        InitializeComponent();


        // Establecer DataContext y activar ViewModel
        this.WhenActivated(disposable =>
        {
            this.DataContext = ViewModel;
            if (ViewModel != null)
            {
                ViewModel.Activator.Activate().DisposeWith(disposable);
            }
        });
    }

    // Manejador para actualizar la propiedad Password
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
        {
            ViewModel.Password = ((PasswordBox)sender).Password;
        }
    }
}
