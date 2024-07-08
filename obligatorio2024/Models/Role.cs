using System;
using System.Collections.Generic;

namespace obligatorio2024.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<Permiso> IdPermisos { get; set; } = new List<Permiso>();

    public virtual ICollection<RolPermiso> RolPermiso { get; set; } = new List<RolPermiso>();



}
