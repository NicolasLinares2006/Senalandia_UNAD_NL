using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Tarjeta
{
    public int IdTarjeta { get; set; }

    public int IdModulo { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? ImagenUrl { get; set; }

    public string? VideoUrl { get; set; }

    public int Orden { get; set; }

    public int? Puntos { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual Modulo IdModuloNavigation { get; set; } = null!;

    public virtual ICollection<ProgresoTarjeta> ProgresoTarjeta { get; set; } = new List<ProgresoTarjeta>();
}
