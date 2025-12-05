using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Observacione
{
    public int IdObservacion { get; set; }

    public int IdTutor { get; set; }

    public int IdEstudiante { get; set; }

    public string Contenido { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;

    public virtual Tutore IdTutorNavigation { get; set; } = null!;
}
