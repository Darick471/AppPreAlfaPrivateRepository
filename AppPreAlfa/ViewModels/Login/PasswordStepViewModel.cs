using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ReactiveUI;

namespace AppPreAlfa.ViewModels.Login;

public class PasswordStepViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    private readonly string _nombreUsuario;
    private string _password;
    private string _warningMessage;
    private bool _isValidating;

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string WarningMessage
    {
        get => _warningMessage;
        set => this.RaiseAndSetIfChanged(ref _warningMessage, value);
    }

    public bool IsValidating
    {
        get => _isValidating;
        set => this.RaiseAndSetIfChanged(ref _isValidating, value);
    }

    public ReactiveCommand<Unit, Unit> SiguienteCommand { get; private set; }

    // el evento ahora se define para que pueda pasar un string (la contraseña).
    public event Action<string> OnPasswordSuccess;

    public PasswordStepViewModel(string nombreUsuario)
    {
        _nombreUsuario = nombreUsuario;

        this.WhenActivated((CompositeDisposable disposables) =>
        {
            var canExecute = this.WhenAnyValue(x => x.Password, p => !string.IsNullOrEmpty(p) && !IsValidating);
            SiguienteCommand = ReactiveCommand.CreateFromTask(ValidarPasswordAsync, canExecute);
            SiguienteCommand.IsExecuting.BindTo(this, x => x.IsValidating);
            SiguienteCommand.ThrownExceptions.Subscribe(ex => WarningMessage = "Error al validar contraseña.");
            SiguienteCommand.DisposeWith(disposables);
        });
    }

    private async Task ValidarPasswordAsync()
    {
        WarningMessage = string.Empty;
        bool esValida = await AuthPasswordAsync(_nombreUsuario, Password.Trim());

        if (esValida)
        {
            // al invocar el evento, pasamos la contraseña validada.
            OnPasswordSuccess?.Invoke(Password.Trim());
        }
        else
        {
            WarningMessage = "Contraseña incorrecta, intente de nuevo.";
        }
    }

    private async Task<bool> AuthPasswordAsync(string nombres, string password)
    {
        try
        {
            using var httpClient = new HttpClient();
            var clienteData = new { token_operacion = 3, nombres, contrasena = password };
            var json = JsonSerializer.Serialize(clienteData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://clouddatacancun.com/logindeprueba.php", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);
            return apiResponse?.status == "success";
        }
        catch
        {
            return false;
        }
    }

    private class ApiResponse
    {
        public string status { get; set; }
    }
}
