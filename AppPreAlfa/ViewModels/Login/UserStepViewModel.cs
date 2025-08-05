using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers; // Necesario para [Reactive] y [ObservableAsProperty]

namespace AppPreAlfa.ViewModels.Login;

public class UserStepViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    [Reactive] public string Nombres { get; set; }
    [Reactive] public string WarningMessage { get; set; }

    // Se notifica el éxto
    [ObservableAsProperty] public bool LoginSuccess { get; }

    public ReactiveCommand<Unit, bool> SiguienteCommand { get; }

    public UserStepViewModel()
    {
        var canExecute = this.WhenAnyValue(x => x.Nombres, n => !string.IsNullOrWhiteSpace(n));
        SiguienteCommand = ReactiveCommand.CreateFromTask(ValidarUsuarioAsync, canExecute);
        SiguienteCommand.ToPropertyEx(this, x => x.LoginSuccess);

        SiguienteCommand.ThrownExceptions.Subscribe(ex => WarningMessage = "Error al validar usuario.");
    }

    private async Task<bool> ValidarUsuarioAsync()
    {
        WarningMessage = string.Empty;
        bool esValido = await AuthUsuarioAsync(Nombres.Trim());
        if (!esValido)
        {
            WarningMessage = "Usuario no encontrado.";
        }
        return esValido;
    }

    private async Task<bool> AuthUsuarioAsync(string nombres)
    {
        try
        {
            using var httpClient = new HttpClient();
            var clienteData = new { token_operacion = 2, nombres };
            var json = JsonSerializer.Serialize(clienteData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://clouddatacancun.com/logindeprueba.php", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseBody);
            return apiResponse?.status == "success";
        }
        catch { return false; }
    }
    private class ApiResponse { public string status { get; set; } }
}
