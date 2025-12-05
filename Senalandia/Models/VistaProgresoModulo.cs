using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class VistaProgresoModulo
{
    public int IdEstudiante { get; set; }

    public int IdModulo { get; set; }

    public string Modulo { get; set; } = null!;

    public int? TotalTarjetas { get; set; }

    public int? TarjetasCompletadas { get; set; }

    public decimal? PorcentajeCompletado { get; set; }
}
