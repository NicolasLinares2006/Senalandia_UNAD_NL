using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Tutore
{
    public int IdTutor { get; set; }

    public int IdUsuario { get; set; }

    public string? Especialidad { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaVinculacion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Observacione> Observaciones { get; set; } = new List<Observacione>();

    public virtual ICollection<TutorEstudiante> TutorEstudiantes { get; set; } = new List<TutorEstudiante>();
}
