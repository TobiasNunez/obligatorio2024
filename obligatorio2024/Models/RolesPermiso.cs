using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class RolesPermiso
{
    public int Id { get; set; }

    public int? RolId { get; set; }

    public string Permiso { get; set; } = null!;

    public virtual Role? Rol { get; set; }
}
