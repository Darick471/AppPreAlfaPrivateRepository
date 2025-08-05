using ReactiveUI;

namespace AppPreAlfa.Helpers;

public class SessionService : ReactiveObject
{
    private static SessionService _instance;
    public static SessionService Instance => _instance ??= new SessionService();

    private int? _usuarioId;
    public int? UsuarioId
    {
        get => _usuarioId;
        set => this.RaiseAndSetIfChanged(ref _usuarioId, value);
    }

    public void ClearSession()
    {
        UsuarioId = null;
    }
}
