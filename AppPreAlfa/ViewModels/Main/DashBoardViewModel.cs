using ReactiveUI;
using AppPreAlfa.Helpers;
using AppPreAlfa.ViewModels.Login;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Disposables;

namespace AppPreAlfa.ViewModels.Main;
// De momento la página está vacía, sirve de camino hacia UserInfoViewModel y Logout
public class DashBoardViewModel : ReactiveObject, IRoutableViewModel
{
    public string UrlPathSegment => "dashboard";
    public IScreen HostScreen { get; }

    public int? UserId => SessionService.Instance.UsuarioId;

    public ReactiveCommand<Unit, IRoutableViewModel> NavigateToUserInfoCommand { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> LogoutCommand { get; }

    public DashBoardViewModel(IScreen hostScreen = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

        NavigateToUserInfoCommand = ReactiveCommand.CreateFromObservable(() =>
            HostScreen.Router.Navigate.Execute(new UserInfoViewModel() as IRoutableViewModel)
        );

        LogoutCommand = ReactiveCommand.CreateFromObservable(() =>
        {
            SessionService.Instance.ClearSession();
            return HostScreen.Router.Navigate.Execute(new LoginFlowViewModel() as IRoutableViewModel);
        });
    }
}
