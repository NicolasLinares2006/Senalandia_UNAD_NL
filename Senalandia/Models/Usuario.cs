using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string TipoUsuario { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public DateTime? UltimaConexion { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();

    public virtual ICollection<Notificacione> Notificaciones { get; set; } = new List<Notificacione>();

    public virtual ICollection<Tutore> Tutores { get; set; } = new List<Tutore>();
}
