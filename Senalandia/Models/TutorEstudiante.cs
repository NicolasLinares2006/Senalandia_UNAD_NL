using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class TutorEstudiante
{
    public int IdAsignacion { get; set; }

    public int IdTutor { get; set; }

    public int IdEstudiante { get; set; }

    public DateTime? FechaAsignacion { get; set; }

    public string? Estado { get; set; }

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;

    public virtual Tutore IdTutorNavigation { get; set; } = null!;
}
