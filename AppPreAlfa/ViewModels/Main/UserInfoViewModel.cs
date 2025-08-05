using System.Reactive.Linq;
using ReactiveUI;
using AppPreAlfa.Models;
using System.Net.Http;
using System.Text.Json;
using AppPreAlfa.Helpers;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System;
using System.Reactive.Disposables;
using Splat;

namespace AppPreAlfa.ViewModels.Main;

public class UserInfoViewModel : ReactiveObject, IActivatableViewModel, IRoutableViewModel
{
    private readonly HttpClient _httpClient = new();
    private UserResponse _userData;

    public ViewModelActivator Activator { get; } = new();
    public string UrlPathSegment => "user-info";
    public IScreen HostScreen { get; }

    public UserResponse UserData
    {
        get => _userData;
        private set => this.RaiseAndSetIfChanged(ref _userData, value);
    }

    public ReactiveCommand<Unit, Unit> LoadUserDataCommand { get; }

    public UserInfoViewModel()
    {
        HostScreen = HostScreen ?? Locator.Current.GetService<IScreen>();
        LoadUserDataCommand = ReactiveCommand.CreateFromTask(LoadUserDataAsync);

        this.WhenActivated(disposables =>
        {
            LoadUserDataCommand.Execute().Subscribe().DisposeWith(disposables);
        });
    }

    private async Task LoadUserDataAsync()
    {
        try
        {
            var request = new
            {
                token_operacion = 5,
                id = SessionService.Instance.UsuarioId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "https://clouddatacancun.com/logindeprueba.php",
                content
            );

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<UserResponse>(responseBody);

            if (apiResponse?.Status == "success")
            {
                UserData = apiResponse;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading user data: {ex.Message}");
        }
    }
}
