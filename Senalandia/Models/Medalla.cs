using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Medalla
{
    public int IdMedalla { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Icono { get; set; }

    public string? Criterio { get; set; }

    public int? PuntosRequeridos { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<EstudianteMedalla> EstudianteMedallas { get; set; } = new List<EstudianteMedalla>();
}
