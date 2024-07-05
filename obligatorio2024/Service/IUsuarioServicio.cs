using obligatorio2024.Models;

namespace obligatorio2024.Service
{
    public interface IUsuarioServicio
    {

        Task<Usuario> GetUsuario(string email, string contrasena);
        Task<Usuario> GetUsuarioById(int id);
        Task<List<string>> ObtenerPermisosPorRol(int idRol);

    }
}
