using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class ActividadEstudiante
{
    public int IdActividad { get; set; }

    public int IdEstudiante { get; set; }

    public DateTime Fecha { get; set; }

    public int? TiempoMinutos { get; set; }

    public int? LeccionesCompletadas { get; set; }

    public int? PuntosGanados { get; set; }

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;
}
