using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class ProgresoTarjeta
{
    public int IdProgreso { get; set; }

    public int IdEstudiante { get; set; }

    public int IdTarjeta { get; set; }

    public bool? Completada { get; set; }

    public DateTime? FechaCompletada { get; set; }

    public int? TiempoDedicadoMinutos { get; set; }

    public int? Intentos { get; set; }

    public DateTime? UltimaVisualizacion { get; set; }

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;

    public virtual Tarjeta IdTarjetaNavigation { get; set; } = null!;
}
