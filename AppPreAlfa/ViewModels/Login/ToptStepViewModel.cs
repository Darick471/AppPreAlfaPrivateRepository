using ReactiveUI;
using System;
using System.Net.Http;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using AppPreAlfa.Helpers;
using System.Linq;
using System.Reactive.Linq;

namespace AppPreAlfa.ViewModels.Login;

public class ToptStepViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    private readonly string _usuario;
    private readonly string _password;

    private string _warningMessage;
    public string WarningMessage { get => _warningMessage; set => this.RaiseAndSetIfChanged(ref _warningMessage, value); }

    private bool _isValidating;
    public bool IsValidating { get => _isValidating; set => this.RaiseAndSetIfChanged(ref _isValidating, value); }

    private string[] _codeParts = new string[6];
    public string[] CodeParts { get => _codeParts; set => this.RaiseAndSetIfChanged(ref _codeParts, value); }

    public ReactiveCommand<Unit, Unit> ValidateToptCommand { get; }
    public event Action OnToptSuccess;

    public ToptStepViewModel(string usuario, string password)
    {
        _usuario = usuario;
        _password = password;

        var canValidate = this.WhenAnyValue(
            x => x.CodeParts,
            x => x.IsValidating,
            (parts, validating) =>
                !validating &&
                (parts?.All(p => !string.IsNullOrEmpty(p)) ?? false) &&
                string.Join("", parts).Length == 6
        );

        ValidateToptCommand = ReactiveCommand.CreateFromTask(ValidarTotpAsync, canValidate);
        ValidateToptCommand.IsExecuting.BindTo(this, x => x.IsValidating);
        ValidateToptCommand.ThrownExceptions.Subscribe(ex => WarningMessage = "Ocurrió un error inesperado.");
    }

    private async Task ValidarTotpAsync()
    {
        WarningMessage = string.Empty;
        var toptCode = string.Join("", CodeParts);
        var (valido, userId) = await AuthTotpAsync(_usuario, _password, toptCode);

        if (valido && userId.HasValue)
        {
            SessionService.Instance.UsuarioId = userId.Value;
            OnToptSuccess?.Invoke();
        }
        else
        {
            WarningMessage = "Código incorrecto, intente de nuevo.";
        }
    }

    private async Task<(bool, int?)> AuthTotpAsync(string nombres, string contrasena, string toptcode)
    {
        try
        {
            using var httpClient = new HttpClient();
            var clienteData = new { token_operacion = 4, nombres, contrasena, toptcode };
            var json = JsonSerializer.Serialize(clienteData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://clouddatacancun.com/logindeprueba.php", content);
            var body = await response.Content.ReadAsStringAsync();
            var apiResp = JsonSerializer.Deserialize<ApiResponse>(body);
            return (apiResp?.status == "success" && apiResp?.id != null, apiResp?.id);
        }
        catch { return (false, null); }
    }

    private class ApiResponse { public string status { get; set; } public int? id { get; set; } }
}
