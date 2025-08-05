using AppPreAlfa.ViewModels.Login;
using Microsoft.UI.Xaml.Controls;
using ReactiveUI;
using Splat;

namespace AppPreAlfa.Views.Login
{
    public sealed partial class LoginPage : Page, IViewFor<LoginFlowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(LoginFlowViewModel), typeof(LoginPage), new PropertyMetadata(null));

        public LoginFlowViewModel ViewModel
        {
            get => (LoginFlowViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (LoginFlowViewModel)value;
        }

        public LoginPage()
        {
            this.InitializeComponent();

            // La vista ya no crea su propio ViewModel.
            // Se lo asignamos directamente para mantener el control.
            ViewModel = new LoginFlowViewModel(Locator.Current.GetService<IScreen>());
            this.DataContext = ViewModel;
        }
    }
}
