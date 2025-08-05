using System;
using System.Diagnostics;
using System.Reactive.Linq;
using AppPreAlfa.Views.Login;
using AppPreAlfa.Views.Main;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using ReactiveUI;
using Splat;

namespace AppPreAlfa.ViewModels.Login;

public class LoginFlowViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
{
    public ViewModelActivator Activator { get; } = new ViewModelActivator();
    public string UrlPathSegment => "LoginFlow";
    public IScreen HostScreen { get; }

    private readonly DispatcherQueue _dispatcher = DispatcherQueue.GetForCurrentThread();

    private string _usuario;
    private string _password;
    private object _currentStepView;

    public object CurrentStepView
    {
        get => _currentStepView;
        private set => this.RaiseAndSetIfChanged(ref _currentStepView, value);
    }

    public LoginFlowViewModel(IScreen hostScreen = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
        CurrentStepView = CreateUserStepView();
    }

    private object CreateUserStepView()
    {
        var userVm = new UserStepViewModel();
        userVm.WhenAnyValue(x => x.LoginSuccess)
              .Where(success => success)
              .Take(1)
              .ObserveOn(RxApp.MainThreadScheduler)
              .Subscribe(_ =>
              {
                  _usuario = userVm.Nombres;
                  CurrentStepView = CreatePasswordStepView(_usuario);
              });
        return new UserStepView { ViewModel = userVm };
    }

    private object CreatePasswordStepView(string nombreUsuario)
    {
        var pwdVm = new PasswordStepViewModel(nombreUsuario);
        pwdVm.OnPasswordSuccess += (password) =>
        {
            _password = password;
            _dispatcher.TryEnqueue(() => CurrentStepView = CreateToptStepView(_usuario, _password));
        };
        return new PasswordStepView { ViewModel = pwdVm };
    }

    private object CreateToptStepView(string usuario, string password)
    {
        var toptVm = new ToptStepViewModel(usuario, password);

        toptVm.OnToptSuccess += () =>
        {
            Debug.WriteLine("Evento OnToptSuccess recibido. Intentando navegar al Dashboard.");
            _dispatcher.TryEnqueue(() =>
            {
                if ((Application.Current as App)?.MainWindow?.Content is Frame rootFrame)
                {
                    rootFrame.Navigate(
                        typeof(DashBoardPage),
                        null,
                        new EntranceNavigationTransitionInfo());
                }
                else
                {
                    Debug.WriteLine("Error: No se pudo encontrar el Frame principal para la navegación.");
                }
            });
        };

        return new ToptStepView { ViewModel = toptVm };
    }
}
