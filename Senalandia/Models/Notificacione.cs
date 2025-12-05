using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Notificacione
{
    public int IdNotificacion { get; set; }

    public int IdUsuario { get; set; }

    public string Tipo { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public string? Contenido { get; set; }

    public bool? Leida { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
