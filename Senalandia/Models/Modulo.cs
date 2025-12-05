using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Modulo
{
    public int IdModulo { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Orden { get; set; }

    public string? Estado { get; set; }

    public string? Color { get; set; }

    public string? Icono { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual ICollection<Tarjeta> Tarjeta { get; set; } = new List<Tarjeta>();
}
