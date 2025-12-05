using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class EstudianteMedalla
{
    public int IdEstudianteMedalla { get; set; }

    public int IdEstudiante { get; set; }

    public int IdMedalla { get; set; }

    public DateTime? FechaObtencion { get; set; }

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;

    public virtual Medalla IdMedallaNavigation { get; set; } = null!;
}
