using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

namespace obligatorio2024.Service
{
    public class UsuarioServicio : IUsuarioServicio
    {

        private readonly Obligatorio2024Context _context;

        public UsuarioServicio(Obligatorio2024Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Usuario> GetUsuario(string email, string contrasena)
        {
            Usuario usuarioEncontrado = await _context.Usuarios.Where(u => u.Email == email && u.Contraseña == contrasena).FirstOrDefaultAsync();

            return usuarioEncontrado;
        }

        public async Task<Usuario> GetUsuarioById(int id)
        {
            Usuario usuarioEncontrado = await _context.Usuarios.Where(u => u.Id == id).FirstOrDefaultAsync();

            return usuarioEncontrado;
        }
        public async Task<List<string>> ObtenerPermisosPorRol(int idRol)
        {
            // Lógica para obtener permisos por el idRol
            var permisos = await _context.Roles
                                .Where(r => r.Id == idRol)
                                .SelectMany(r => r.RolesPermisos)
                                .Select(rp => rp.Permiso.NombrePermiso)
                                .ToListAsync();

            return permisos;
        }
    }
}
