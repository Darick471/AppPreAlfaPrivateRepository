namespace AppPreAlfa.Models;

public class UserResponse
{
    public string Status { get; set; }
    public string Message { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string Correo { get; set; }
    public string Telefono { get; set; }
    public UserFile[] Archivos { get; set; }
}

public class UserFile
{
    public string Filename { get; set; }
    public string Filetype { get; set; }
    public string Filesize { get; set; }
    public string UploadedAt { get; set; }
}
