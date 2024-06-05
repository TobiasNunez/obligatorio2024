using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string? Telefono { get; set; }

    public int? RolId { get; set; }

    public virtual Role? Rol { get; set; }
}
